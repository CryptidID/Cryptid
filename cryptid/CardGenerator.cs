using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using Cryptid;
using iTextSharp.text.pdf;
using Cryptid.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using CSRectangle = System.Drawing.Rectangle;
using CSImage = System.Drawing.Image;
using CSImaging = System.Drawing.Imaging;
using Image = iTextSharp.text.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace cryptid {
    public class CardGenerator {
        private readonly string _pdfTemplateFile;
        private readonly Candidate _cardCandidate;

        public CardGenerator(Candidate cardCandidate, string pdfTemplateFile) {
            _cardCandidate = cardCandidate;
            _pdfTemplateFile = pdfTemplateFile;
        }

        public void Generate(string outputFile, byte[] chainId) {
            if (_cardCandidate.Uid == null) throw new Exception("A UID is required to generate a new card!");

            PdfReader reader = new PdfReader("cryptid-id-template.pdf");

            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(outputFile, FileMode.Create))) {
                AcroFields form = stamper.AcroFields;
                var keys = form.Fields.Keys;

                // fill in form
                foreach (string key in keys) {
                    if (key.Contains("UID")) {
                        form.SetField(key, Arrays.ByteArrayToHex(_cardCandidate.Uid));
                    } else if (key.Contains("LN")) {
                        form.SetField(key, _cardCandidate.Dcs);
                    } else if (key.Contains("FN_MN")) {
                        form.SetField(key, _cardCandidate.Dac + ", " + _cardCandidate.Dad);
                    } else if (key.Contains("SEX")) {
                        form.SetField(key, _cardCandidate.Dbc == Candidate.Sex.Male ? "M" : "F");
                    } else if (key.Contains("EYE")) {
                        form.SetField(key, _cardCandidate.Day.ANSIFormat());
                    } else if (key.Contains("HT")) {
                        form.SetField(key, _cardCandidate.Dau.ANSIFormat.TrimStart('0'));
                    } else if (key.Contains("DOB")) {
                        form.SetField(key, _cardCandidate.Dbb.ToString("MM/dd/yyyy"));
                    } else if (key.Contains("ISSUED")) {
                        form.SetField(key, _cardCandidate.Dbd.ToString("MM/dd/yyyy"));
                    } else if (key.Contains("ADDR_1")) {
                        form.SetField(key, _cardCandidate.Dag);
                    } else if (key.Contains("ADDR_2")) {
                        form.SetField(key, _cardCandidate.Dai + ", " + _cardCandidate.Daj + " " + _cardCandidate.Dcg + " " + _cardCandidate.Dak.ANSIFormat.TrimEnd('0'));
                    } else if (key.Contains("CRC")) {
                        form.SetField(key, Arrays.ByteArrayToHex(new Crypto.Crc32().ComputeHash(_cardCandidate.Uid)));
                    }
                }

                stamper.FormFlattening = true;
                PdfContentByte pdfContentByte = stamper.GetOverContent(1);

                var size = reader.GetPageSize(1);

                // Add qr code
                BarcodeQRCode qr = new BarcodeQRCode(Convert.ToBase64String(chainId), 1000, 1000, null);
                Image img = qr.GetImage();
                img.ScalePercent(300);

                Bitmap qrBmp = ImageToBitmap(img);
                qrBmp = TrimBitmap(qrBmp);
                img = Image.GetInstance(qrBmp, (BaseColor)null);
                img.ScaleAbsolute((1300f / 4050f) * size.Width, (1300f / 2550f) * size.Height);

                Image mask = qr.GetImage();
                mask.MakeMask();
                img.ImageMask = mask;
                img.SetAbsolutePosition((2755f / 4050f) * size.Width, (450f / 2550f) * size.Height);
                pdfContentByte.AddImage(img);

                // Add headshot
                img = Image.GetInstance(_cardCandidate.Image, (BaseColor) null);
                img.ScaleAbsolute((int)((1112f / 4050f) * size.Width), (int)((1484f / 2550f) * size.Height));
                img.SetAbsolutePosition((100f / 4050f) * size.Width, (130f / 2550f) * size.Height);
                pdfContentByte.AddImage(img);
            }
        }

        public Bitmap ImageToBitmap(Image img) {
            var bmp = new Bitmap((int) img.Width, (int) img.Height, PixelFormat.Format24bppRgb);
            CSRectangle rect = new CSRectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpdat = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            Marshal.Copy(img.RawData, 0, bmpdat.Scan0, img.RawData.Length);
            bmp.UnlockBits(bmpdat);
            return bmp;
        }

        private Bitmap TrimBitmap(Bitmap source) {
            CSRectangle srcRect = default(CSRectangle);
            BitmapData data = null;
            try {
                data = source.LockBits(new CSRectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                byte[] buffer = new byte[data.Height * data.Stride];
                Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
                int xMin = int.MaxValue;
                int xMax = 0;
                int yMin = int.MaxValue;
                int yMax = 0;
                for (int y = 0; y < data.Height; y++) {
                    for (int x = 0; x < data.Width; x++) {
                        byte alpha = buffer[y * data.Stride + 4 * x + 3];
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
            } finally {
                if (data != null)
                    source.UnlockBits(data);
            }

            Bitmap dest = new Bitmap(srcRect.Width, srcRect.Height);
            CSRectangle destRect = new CSRectangle(0, 0, srcRect.Width, srcRect.Height);
            using (Graphics graphics = Graphics.FromImage(dest)) {
                graphics.DrawImage(source, destRect, srcRect, GraphicsUnit.Pixel);
            }
            return dest;
        }
    }
}
