using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using Cryptid.Utils;
using MsgPack;
using MsgPack.Serialization;
using SourceAFIS.Simple;

namespace Cryptid {
    public static class Extensions {
        public static string ANSIFormat(this Candidate.EyeColor ec) {
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

        private const int MAP_COUNT = 15;
        private const string DATE_FORMAT = "MM/dd/yyyy";

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
        public Image Image { get; set; }
        public Fingerprint Fingerprint { get; set; }

        public byte[] Serialize() {
            using (MemoryStream ms = new MemoryStream()) {
                var serializer = MessagePackSerializer.Get<Candidate>();
                serializer.Pack(ms, this);
                return ms.ToArray();
            }
        }

        public static Candidate Deserialize(byte[] b) {
            var serializer = MessagePackSerializer.Get<Candidate>();
            return serializer.Unpack(new MemoryStream(b));
        }

        public void PackToMessage(Packer packer, PackingOptions options) {
            // pack the header for the amount of items in the map
            packer.PackMapHeader(MAP_COUNT);

            packer.Pack("DCS");
            packer.Pack(Dcs);

            packer.Pack("DAC");
            packer.Pack(Dac);

            packer.Pack("DAD");
            packer.Pack(Dad);

            packer.Pack("DBD");
            packer.Pack(Dbd.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));

            packer.Pack("DBB");
            packer.Pack(Dbb.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));

            packer.Pack("DBC");
            packer.Pack((int) Dbc);

            packer.Pack("DAY");
            packer.Pack(Day.ANSIFormat());

            packer.Pack("DAU");
            packer.Pack(Dau.ANSIFormat);

            packer.Pack("DAG");
            packer.Pack(Dag);

            packer.Pack("DAI");
            packer.Pack(Dai);

            packer.Pack("DAJ");
            packer.Pack(Daj);

            packer.Pack("DAK");
            packer.Pack(Dak.ANSIFormat);

            packer.Pack("DCG");
            packer.Pack(Dcg);

            // pack image
            packer.Pack("ZAA");
            var imageConverter = new ImageConverter();
            packer.Pack((byte[]) imageConverter.ConvertTo(Image, typeof (byte[])));

            // pack fingerprint
            packer.Pack("ZAB");

            if (Fingerprint.Image != null) {
                AfisEngine afis = new AfisEngine();
                Person p = new Person(Fingerprint);
                afis.Extract(p);
            }
            if (Fingerprint.AsIsoTemplate != null) packer.Pack(Fingerprint.AsIsoTemplate);
        }

        public void UnpackFromMessage(Unpacker unpacker) {
            string dcs, dac, dad, dbd, dbb, day, dau, dag, dai, daj, dak, dcg;
            int dbc;

            if (!unpacker.IsMapHeader) throw SerializationExceptions.NewIsNotMapHeader();

            if (UnpackHelpers.GetItemsCount(unpacker) != MAP_COUNT)
                throw SerializationExceptions.NewUnexpectedArrayLength(MAP_COUNT, UnpackHelpers.GetItemsCount(unpacker));

            for (var i = 0; i < MAP_COUNT; i++) {
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
                        Dau = new Height();
                        Dau.ANSIFormat = dau;
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
                        Dak = new PostalCode();
                        Dak.ANSIFormat = dak;
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
                        Fingerprint = new Fingerprint();
                        Fingerprint.AsIsoTemplate = unpacker.LastReadData.AsBinary();
                        break;
                    }
                }
            }
        }

        public bool IsComplete() {
            if (string.IsNullOrWhiteSpace(Dcs)) return false;
            if (string.IsNullOrWhiteSpace(Dac)) return false;
            if (string.IsNullOrWhiteSpace(Dad)) return false;
            if (string.IsNullOrWhiteSpace(Dag)) return false;
            if (string.IsNullOrWhiteSpace(Dai)) return false;
            if (string.IsNullOrWhiteSpace(Daj)) return false;
            if (string.IsNullOrWhiteSpace(Dcg)) return false;
            if (Image == null) return false;
            if (Fingerprint.AsIsoTemplate == null || Fingerprint.AsBitmap == null) return false;
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
                if (val.Equals(s)) return ec;
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

            public string ANSIFormat {
                set {
                    if (!value.Contains(" ")) throw new ArgumentException("value is not in ANSI format!");
                    value = value.Split(' ')[0];

                    var inches = -1;
                    if (!int.TryParse(value, out inches)) throw new ArgumentException("value is not in ANSI format!");
                    Feet = inches/12;
                    Inches = inches - (Feet*12);
                }
                get { return (((Feet*12) + Inches).ToString().PadLeft(3, '0') + " IN"); }
            }
        }

        public class PostalCode {
            private string _postCode;

            public PostalCode() {
            }

            public PostalCode(string postalCode) {
                ANSIFormat = postalCode;
            }

            public string ANSIFormat {
                get { return _postCode; }
                set {
                    if (value.Length > 9)
                        throw new ArgumentOutOfRangeException("Postal code must be 9 characters or less.");
                    if (value == null) throw new ArgumentNullException("value");
                    _postCode = value.PadRight(9, '0');
                }
            }
        }
    }
}