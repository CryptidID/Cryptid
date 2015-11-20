#region

using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Cryptid;
using Cryptid.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using CSRectangle = System.Drawing.Rectangle;
using CSImage = System.Drawing.Image;
using CSImaging = System.Drawing.Imaging;
using Image = iTextSharp.text.Image;

#endregion

namespace cryptid {
    public class CardGenerator {
        private readonly Candidate _cardCandidate;
        private readonly string _pdfTemplateFile;

        public CardGenerator(Candidate cardCandidate, string pdfTemplateFile) {
            _cardCandidate = cardCandidate;
            _pdfTemplateFile = pdfTemplateFile;
        }

        public void Generate(string outputFile, byte[] chainId) {
            if (_cardCandidate.Uid == null) throw new Exception("A UID is required to generate a new card!");

            var reader = new PdfReader(_pdfTemplateFile);

            using (var stamper = new PdfStamper(reader, new FileStream(outputFile, FileMode.Create))) {
                var form = stamper.AcroFields;
                var keys = form.Fields.Keys;

                // fill in form
                foreach (var key in keys) {
                    if (key.Contains("UID")) {
                        form.SetField(key, Arrays.ByteArrayToHex(_cardCandidate.Uid));
                    }
                    else if (key.Contains("LN")) {
                        form.SetField(key, _cardCandidate.Dcs);
                    }
                    else if (key.Contains("FN_MN")) {
                        form.SetField(key, _cardCandidate.Dac + ", " + _cardCandidate.Dad);
                    }
                    else if (key.Contains("SEX")) {
                        form.SetField(key, _cardCandidate.Dbc == Candidate.Sex.Male ? "M" : "F");
                    }
                    else if (key.Contains("EYE")) {
                        form.SetField(key, _cardCandidate.Day.AnsiFormat());
                    }
                    else if (key.Contains("HT")) {
                        form.SetField(key, _cardCandidate.Dau.AnsiFormat.TrimStart('0'));
                    }
                    else if (key.Contains("DOB")) {
                        form.SetField(key, _cardCandidate.Dbb.ToString("MM/dd/yyyy"));
                    }
                    else if (key.Contains("ISSUED")) {
                        form.SetField(key, _cardCandidate.Dbd.ToString("MM/dd/yyyy"));
                    }
                    else if (key.Contains("ADDR_1")) {
                        form.SetField(key, _cardCandidate.Dag);
                    }
                    else if (key.Contains("ADDR_2")) {
                        form.SetField(key,
                            _cardCandidate.Dai + ", " + _cardCandidate.Daj + " " + _cardCandidate.Dcg + " " +
                            _cardCandidate.Dak.AnsiFormat.TrimEnd('0'));
                    }
                    else if (key.Contains("CRC")) {
                        form.SetField(key, Arrays.ByteArrayToHex(new Crypto.Crc32().ComputeHash(_cardCandidate.Uid)));
                    }
                }

                stamper.FormFlattening = true;
                var pdfContentByte = stamper.GetOverContent(1);

                var size = reader.GetPageSize(1);

                // Add qr code
                var qr = new BarcodeQRCode(Convert.ToBase64String(chainId), 1000, 1000, null);
                var img = qr.GetImage();
                img.ScalePercent(300);

                var qrBmp = ImageToBitmap(img);
                qrBmp = TrimBitmap(qrBmp);
                img = Image.GetInstance(qrBmp, (BaseColor) null);
                img.ScaleAbsolute(1300f/4050f*size.Width, 1300f/2550f*size.Height);

                var mask = qr.GetImage();
                mask.MakeMask();
                img.ImageMask = mask;
                img.SetAbsolutePosition(2780f/4050f*size.Width, 450f/2550f*size.Height);
                pdfContentByte.AddImage(img);

                // Add headshot
                img = Image.GetInstance(_cardCandidate.Image, (BaseColor) null);
                img.ScaleAbsolute((int) (1112f/4050f*size.Width), (int) (1484f/2550f*size.Height));
                img.SetAbsolutePosition(100f/4050f*size.Width, 130f/2550f*size.Height);
                pdfContentByte.AddImage(img);
            }
        }

        public Bitmap ImageToBitmap(Image img) {
            var bmp = new Bitmap((int) img.Width, (int) img.Height, CSImaging.PixelFormat.Format24bppRgb);
            var rect = new CSRectangle(0, 0, bmp.Width, bmp.Height);
            var bmpdat = bmp.LockBits(rect, CSImaging.ImageLockMode.ReadWrite, CSImaging.PixelFormat.Format24bppRgb);
            Marshal.Copy(img.RawData, 0, bmpdat.Scan0, img.RawData.Length);
            bmp.UnlockBits(bmpdat);
            return bmp;
        }

        private Bitmap TrimBitmap(Bitmap source) {
            CSRectangle srcRect;
            CSImaging.BitmapData data = null;
            try {
                data = source.LockBits(new CSRectangle(0, 0, source.Width, source.Height),
                    CSImaging.ImageLockMode.ReadOnly, CSImaging.PixelFormat.Format32bppArgb);
                var buffer = new byte[data.Height*data.Stride];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                var xMin = int.MaxValue;
                var xMax = 0;
                var yMin = int.MaxValue;
                var yMax = 0;
                for (var y = 0; y < data.Height; y++) {
                    for (var x = 0; x < data.Width; x++) {
                        var alpha = buffer[y*data.Stride + 4*x + 3];
                        if (alpha != 0) {
                            if (x < xMin) xMin = x;
                            if (x > xMax) xMax = x;
                            if (y < yMin) yMin = y;
                            if (y > yMax) yMax = y;
                        }
                    }
                }
                if (xMax < xMin || yMax < yMin) {
                    return null;
                }
                srcRect = CSRectangle.FromLTRB(xMin, yMin, xMax, yMax);
            }
            finally {
                if (data != null)
                    source.UnlockBits(data);
            }

            var dest = new Bitmap(srcRect.Width, srcRect.Height);
            var destRect = new CSRectangle(0, 0, srcRect.Width, srcRect.Height);
            using (var graphics = Graphics.FromImage(dest)) {
                graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return dest;
        }
    }
}