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
        /// <summary>
        ///     Extension method to get the ANSI format of an EyeColor enum
        /// </summary>
        /// <param name="ec">The EyeColor enum</param>
        /// <returns>The ANSI format of this eye color</returns>
        public static string AnsiFormat(this Candidate.EyeColor ec) {
            string output = null;
            var type = ec.GetType();
            var fi = type.GetField(ec.ToString());
            var attrs = fi.GetCustomAttributes(typeof (StringValue), false) as StringValue[];
            if (attrs != null && attrs.Length > 0) output = attrs[0].Value;
            return output;
        }
    }

    /// <summary>
    ///     This class holds all an ID holders information.
    ///     In the case of this project, candidate is synonomyous
    ///     with ID holder and client.
    /// </summary>
    public class Candidate : IPackable, IUnpackable {
        /// <summary>
        ///     An enum representing a candidate's eye color
        /// </summary>
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

        /// <summary>
        ///     An enum representing the Sex of a candidate.
        ///     The integer value of the enum is ANSI format.
        /// </summary>
        public enum Sex {
            Male = 1,
            Female = 2
        }

        /// <summary>
        ///     The number of fields stored in the MessagePack object
        /// </summary>
        private const int MapCount = 15;

        /// <summary>
        ///     The ANSI date format
        /// </summary>
        private const string DateFormat = "MM/dd/yyyy";

        /// <summary>
        ///     The unique id of this particular candidate.
        /// </summary>
        public byte[] Uid { get; set; }

        /// <summary>
        ///     Candidate family name
        /// </summary>
        public string Dcs { get; set; }

        /// <summary>
        ///     Candidate first name
        /// </summary>
        public string Dac { get; set; }

        /// <summary>
        ///     Candidate middle name
        /// </summary>
        public string Dad { get; set; }

        /// <summary>
        ///     Document issue date
        /// </summary>
        public DateTime Dbd { get; set; }

        /// <summary>
        ///     Candidate date of birth
        /// </summary>
        public DateTime Dbb { get; set; }

        /// <summary>
        ///     Physical Description - Sex
        /// </summary>
        public Sex Dbc { get; set; }

        /// <summary>
        ///     Physical Description - Eye Color
        /// </summary>
        public EyeColor Day { get; set; }

        /// <summary>
        ///     Physical Description - Height
        /// </summary>
        public Height Dau { get; set; }

        /// <summary>
        ///     Address - Street
        /// </summary>
        public string Dag { get; set; }

        /// <summary>
        ///     Address - City
        /// </summary>
        public string Dai { get; set; }

        /// <summary>
        ///     Address - Jurisdiction Code (state)
        /// </summary>
        public string Daj { get; set; }

        /// <summary>
        ///     Address - Postal code
        /// </summary>
        public PostalCode Dak { get; set; }

        /// <summary>
        ///     Address - Country
        /// </summary>
        public string Dcg { get; set; }

        /// <summary>
        ///     The headshot image of the candidate
        /// </summary>
        [JsonIgnore]
        public Image Image { get; set; }

        /// <summary>
        ///     The headshot image as a base64 string
        /// </summary>
        public string ImageString {
            get {
                using (var ms = new MemoryStream()) {
                    Image.Save(ms, ImageFormat.Jpeg);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        /// <summary>
        ///     The candidates fingerprint
        /// </summary>
        public Fingerprint Fingerprint { get; set; }

        /// <summary>
        ///     Packs the candidate to a MessagePack objects
        ///     This method should not be called directly, use serialize instead.
        /// </summary>
        /// <param name="packer">The packer</param>
        /// <param name="options">The packer options</param>
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

        /// <summary>
        ///     Unpacks the message from a MessagePack object
        ///     This method should not be called directly, use deserialize instead.
        /// </summary>
        /// <param name="unpacker">The unpacker</param>
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

        /// <summary>
        ///     Serializes the candidate to a MessagePack object
        /// </summary>
        /// <returns>The bytes of the MessagePack object</returns>
        public byte[] Serialize() {
            using (var ms = new MemoryStream()) {
                var serializer = MessagePackSerializer.Get<Candidate>();
                serializer.Pack(ms, this);
                return ms.ToArray();
            }
        }

        /// <summary>
        ///     Gets the Candidate object from the MessagePack object bytes
        /// </summary>
        /// <param name="b">The MessagePack object bytes</param>
        /// <returns>The Candidate object</returns>
        public static Candidate Deserialize(byte[] b) {
            var serializer = MessagePackSerializer.Get<Candidate>();
            return serializer.Unpack(new MemoryStream(b));
        }

        /// <summary>
        ///     Checks if a Candidate object is complete enough to be submitted
        /// </summary>
        /// <returns>Whether or not the object is complete</returns>
        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        public bool IsComplete() {
            if (string.IsNullOrWhiteSpace(Dcs)) return false;
            if (string.IsNullOrWhiteSpace(Dac)) return false;
            if (string.IsNullOrWhiteSpace(Dag)) return false;
            if (string.IsNullOrWhiteSpace(Dai)) return false;
            if (string.IsNullOrWhiteSpace(Daj)) return false;
            if (string.IsNullOrWhiteSpace(Dcg)) return false;
            if (Image == null) return false;
            if (Fingerprint.AsIsoTemplate == null && Fingerprint.AsBitmap == null) return false;
            if (Dbd == null || Dbb == null | Dbc == null || Day == null || Dau == null || Dak == null) return false;
            return true;
        }

        /// <summary>
        ///     Gets the EyeColor enum from a string representing the eye color
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     An object representing the height of the candidate
        /// </summary>
        public class Height {
            public Height(int feet, int inches) {
                Feet = feet;
                Inches = inches;
            }

            public Height() {
            }

            public int Feet { get; set; }
            public int Inches { get; set; }

            /// <summary>
            ///     The height of the candidate in ANSI format
            /// </summary>
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

        /// <summary>
        ///     An objet representing the postal code of the candidate
        /// </summary>
        public class PostalCode {
            private string _postCode;

            public PostalCode() {
            }

            public PostalCode(string postalCode) {
                AnsiFormat = postalCode;
            }

            /// <summary>
            ///     The candidate's postal code in ANSI format
            /// </summary>
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