using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptid;
using iTextSharp.text.pdf;
using Cryptid.Utils;
using CSRectangle = System.Drawing.Rectangle;
using CSImage = System.Drawing.Image;
using Image = iTextSharp.text.Image;

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

            PdfReader reader = new PdfReader(_pdfTemplateFile);

            using (PdfStamper stamper = new PdfStamper(reader, new FileStream(outputFile, FileMode.Create))) {
                AcroFields form = stamper.AcroFields;
                var keys = form.Fields.Keys;

                // fill in form
                foreach (string key in keys) {
                    if (key.Contains("UID")) {
                        
                    } else if (key.Contains("UID")) {
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
                        form.SetField(key, _cardCandidate.Dbb.ToString("MM/dd/YY"));
                    } else if (key.Contains("ISSUED")) {
                        form.SetField(key, _cardCandidate.Dbd.ToString("MM/dd/YY"));
                    } else if (key.Contains("ADDR_1")) {
                        form.SetField(key, _cardCandidate.Dag);
                    } else if (key.Contains("ADDR_2")) {
                        form.SetField(key, _cardCandidate.Dai + ", " + _cardCandidate.Daj + " " + _cardCandidate.Dcg + " " + _cardCandidate.Dak);
                    } else if (key.Contains("CRC")) {
                        //TODO: NEED CRC32
                        //form.SetField(key, Arrays.ByteArrayToHex(BitConverter.GetBytes(Crypto.Crc16.ComputeChecksum(_cardCandidate.Uid))));
                    }
                }

                stamper.FormFlattening = true;
                PdfContentByte pdfContentByte = stamper.GetOverContent(1);

                // Add Aztec code
                BarcodeQRCode qr = new BarcodeQRCode(Arrays.ByteArrayToHex(chainId), 975, 975, null);
                Image img = qr.GetImage();
                Image mask = qr.GetImage();
                mask.MakeMask();
                img.ImageMask = mask;
                img.SetAbsolutePosition(2199.75f, 704.25f);
                pdfContentByte.AddImage(img);

                // Add headshot
                img = Image.GetInstance(ResizeImage(_cardCandidate.Image, 1112, 1484), ImageFormat.MemoryBmp);
                img.SetAbsolutePosition(75.75f, 704.25f);
                pdfContentByte.AddImage(img);
            } 
        }

        public static Bitmap ResizeImage(CSImage image, int width, int height) {
            var destRect = new CSRectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
