#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using Cryptid.Utils;
using MsgPack;
using MsgPack.Serialization;
using Newtonsoft.Json;
using SourceAFIS.Simple;

#endregion

namespace Cryptid {
    public static class Extensions {
        public static string AnsiFormat(this Candidate.EyeColor ec) {
            string output = null;
            var type = ec.GetType();
            var fi = type.GetField(ec.ToString());
            var attrs = fi.GetCustomAttributes(typeof (StringValue), false) as StringValue[];
            if (attrs != null && attrs.Length > 0) output = attrs[0].Value;
            return output;
        }
    }

    public class Candidate : IPackable, IUnpackable {
        public enum EyeColor {
            [StringValue("BLK")] Black,

            [StringValue("BLUE")] Blue,

            [StringValue("BRO")] Brown,

            [StringValue("GRY")] Gray,

            [StringValue("GRN")] Green,

            [StringValue("HAZ")] Hazel,

            [StringValue("MAR")] Maroon,

            [StringValue("PNK")] Pink,

            [StringValue("DIC")] Dichromatic,

            [StringValue("UNK")] Unknown
        }

        public enum Sex {
            Male = 1,
            Female = 2
        }

        private const int MapCount = 15;
        private const string DateFormat = "MM/dd/yyyy";

        public byte[] Uid { get; set; }

        public string Dcs { get; set; }
        public string Dac { get; set; }
        public string Dad { get; set; }
        public DateTime Dbd { get; set; }
        public DateTime Dbb { get; set; }
        public Sex Dbc { get; set; }
        public EyeColor Day { get; set; }
        public Height Dau { get; set; }
        public string Dag { get; set; }
        public string Dai { get; set; }
        public string Daj { get; set; }
        public PostalCode Dak { get; set; }
        public string Dcg { get; set; }
        [JsonIgnore]
        public Image Image { get; set; }

        public string ImageString {
            get {
                using (MemoryStream ms = new MemoryStream()) {
                    Image.Save(ms, ImageFormat.Jpeg);
                    byte[] imageBytes = ms.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public Fingerprint Fingerprint { get; set; }

        public void PackToMessage(Packer packer, PackingOptions options) {
            // pack the header for the amount of items in the map
            packer.PackMapHeader(MapCount);

            packer.Pack("DCS");
            packer.Pack(Dcs);

            packer.Pack("DAC");
            packer.Pack(Dac);

            packer.Pack("DAD");
            packer.Pack(Dad);

            packer.Pack("DBD");
            packer.Pack(Dbd.ToString(DateFormat, CultureInfo.InvariantCulture));

            packer.Pack("DBB");
            packer.Pack(Dbb.ToString(DateFormat, CultureInfo.InvariantCulture));

            packer.Pack("DBC");
            packer.Pack((int) Dbc);

            packer.Pack("DAY");
            packer.Pack(Day.AnsiFormat());

            packer.Pack("DAU");
            packer.Pack(Dau.AnsiFormat);

            packer.Pack("DAG");
            packer.Pack(Dag);

            packer.Pack("DAI");
            packer.Pack(Dai);

            packer.Pack("DAJ");
            packer.Pack(Daj);

            packer.Pack("DAK");
            packer.Pack(Dak.AnsiFormat);

            packer.Pack("DCG");
            packer.Pack(Dcg);

            // pack image
            packer.Pack("ZAA");
            var imageConverter = new ImageConverter();
            packer.Pack((byte[]) imageConverter.ConvertTo(Image, typeof (byte[])));

            // pack fingerprint
            packer.Pack("ZAB");

            if (Fingerprint.Image != null) {
                var afis = new AfisEngine();
                var p = new Person(Fingerprint);
                afis.Extract(p);
            }
            if (Fingerprint.AsIsoTemplate != null) packer.Pack(Fingerprint.AsIsoTemplate);
        }

        public void UnpackFromMessage(Unpacker unpacker) {
            string dcs, dac, dad, dbd, dbb, day, dau, dag, dai, daj, dak, dcg;
            int dbc;

            if (!unpacker.IsMapHeader) throw SerializationExceptions.NewIsNotMapHeader();

            if (UnpackHelpers.GetItemsCount(unpacker) != MapCount)
                throw SerializationExceptions.NewUnexpectedArrayLength(MapCount, UnpackHelpers.GetItemsCount(unpacker));

            for (var i = 0; i < MapCount; i++) {
                string key;

                if (!unpacker.ReadString(out key)) throw SerializationExceptions.NewUnexpectedEndOfStream();

                switch (key) {
                    case "DCS": {
                        if (!unpacker.ReadString(out dcs)) throw SerializationExceptions.NewMissingProperty("dcs");
                        Dcs = dcs;
                        break;
                    }
                    case "DAC": {
                        if (!unpacker.ReadString(out dac)) throw SerializationExceptions.NewMissingProperty("dac");
                        Dac = dac;
                        break;
                    }
                    case "DAD": {
                        if (!unpacker.ReadString(out dad)) throw SerializationExceptions.NewMissingProperty("dad");
                        Dad = dad;
                        break;
                    }
                    case "DBD": {
                        if (!unpacker.ReadString(out dbd)) throw SerializationExceptions.NewMissingProperty("dbd");
                        Dbd = DateTime.Parse(dbd);
                        break;
                    }
                    case "DBB": {
                        if (!unpacker.ReadString(out dbb)) throw SerializationExceptions.NewMissingProperty("dbb");
                        Dbb = DateTime.Parse(dbb);
                        break;
                    }
                    case "DBC": {
                        if (!unpacker.ReadInt32(out dbc)) throw SerializationExceptions.NewMissingProperty("dbc");
                        Dbc = (Sex) dbc;
                        break;
                    }
                    case "DAY": {
                        if (!unpacker.ReadString(out day)) throw SerializationExceptions.NewMissingProperty("day");
                        Day = GetEyeColor(day);
                        break;
                    }
                    case "DAU": {
                        if (!unpacker.ReadString(out dau)) throw SerializationExceptions.NewMissingProperty("dau");
                        Dau = new Height {AnsiFormat = dau};
                        break;
                    }
                    case "DAG": {
                        if (!unpacker.ReadString(out dag)) throw SerializationExceptions.NewMissingProperty("dag");
                        Dag = dag;
                        break;
                    }
                    case "DAI": {
                        if (!unpacker.ReadString(out dai)) throw SerializationExceptions.NewMissingProperty("dai");
                        Dai = dai;
                        break;
                    }
                    case "DAJ": {
                        if (!unpacker.ReadString(out daj)) throw SerializationExceptions.NewMissingProperty("daj");
                        Daj = daj;
                        break;
                    }
                    case "DAK": {
                        if (!unpacker.ReadString(out dak)) throw SerializationExceptions.NewMissingProperty("dak");
                        Dak = new PostalCode {AnsiFormat = dak};
                        break;
                    }
                    case "DCG": {
                        if (!unpacker.ReadString(out dcg)) throw SerializationExceptions.NewMissingProperty("dcg");
                        Dcg = dcg;
                        break;
                    }
                    case "ZAA": {
                        if (!unpacker.Read()) throw SerializationExceptions.NewMissingProperty("zaa");
                        var ms = new MemoryStream(unpacker.LastReadData.AsBinary());
                        Image = Image.FromStream(ms);
                        break;
                    }
                    case "ZAB": {
                        if (!unpacker.Read()) throw SerializationExceptions.NewMissingProperty("zab");
                        Fingerprint = new Fingerprint {AsIsoTemplate = unpacker.LastReadData.AsBinary()};
                        break;
                    }
                }
            }
        }

        public byte[] Serialize() {
            using (var ms = new MemoryStream()) {
                var serializer = MessagePackSerializer.Get<Candidate>();
                serializer.Pack(ms, this);
                return ms.ToArray();
            }
        }

        public static Candidate Deserialize(byte[] b) {
            var serializer = MessagePackSerializer.Get<Candidate>();
            return serializer.Unpack(new MemoryStream(b));
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public bool IsComplete() {
            if (string.IsNullOrWhiteSpace(Dcs)) return false;
            if (string.IsNullOrWhiteSpace(Dac)) return false;
            //if (string.IsNullOrWhiteSpace(Dad)) return false;
            if (string.IsNullOrWhiteSpace(Dag)) return false;
            if (string.IsNullOrWhiteSpace(Dai)) return false;
            if (string.IsNullOrWhiteSpace(Daj)) return false;
            if (string.IsNullOrWhiteSpace(Dcg)) return false;
            if (Image == null) return false;
            if (Fingerprint.AsIsoTemplate == null && Fingerprint.AsBitmap == null) return false;
            if (Dbd == null || Dbb == null | Dbc == null || Day == null || Dau == null || Dak == null) return false;
            return true;
        }

        public EyeColor GetEyeColor(string s) {
            foreach (EyeColor ec in Enum.GetValues(typeof (EyeColor))) {
                string val = null;
                var type = ec.GetType();
                var fi = type.GetField(ec.ToString());
                var attrs = fi.GetCustomAttributes(typeof (StringValue), false) as StringValue[];
                if (attrs != null && attrs.Length > 0) val = attrs[0].Value;
                if (val != null && val.Equals(s)) return ec;
            }
            return EyeColor.Unknown;
        }

        public class Height {
            public Height(int feet, int inches) {
                Feet = feet;
                Inches = inches;
            }

            public Height() {
            }

            public int Feet { get; set; }
            public int Inches { get; set; }

            public string AnsiFormat {
                set {
                    if (!value.Contains(" ")) throw new ArgumentException("Value is not in ANSI format!");
                    value = value.Split(' ')[0];

                    int inches;
                    if (!int.TryParse(value, out inches)) throw new ArgumentException("Value is not in ANSI format!");
                    Feet = inches/12;
                    Inches = inches - Feet*12;
                }
                get { return (Feet*12 + Inches).ToString().PadLeft(3, '0') + " IN"; }
            }
        }

        public class PostalCode {
            private string _postCode;

            public PostalCode() {
            }

            public PostalCode(string postalCode) {
                AnsiFormat = postalCode;
            }

            public string AnsiFormat {
                get { return _postCode; }
                set {
                    if (value.Length > 9)
                        throw new ArgumentOutOfRangeException(@"Postal code must be 9 characters or less.");
                    if (value == null) throw new ArgumentNullException(nameof(value));
                    _postCode = value.PadRight(9, '0');
                }
            }
        }
    }
}