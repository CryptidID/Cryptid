using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using cryptid.Factom.API;

namespace Cryptid.Utils {


    public static class Keys {
        public static RSAParameters PrivateKey(string path) {
            var k = new RSACryptoServiceProvider();
            k.FromXmlString(File.ReadAllText(path));
            return k.ExportParameters(true);
        }


        public static RSAParameters PublicKey(string path) {
            var k = new RSACryptoServiceProvider();
            k.FromXmlString(File.ReadAllText(path));
            return k.ExportParameters(false);
        }
    }

    public class StringValue : Attribute {
        public StringValue(string value) {
            Value = value;
        }

        public string Value { get; }
    }


    public static class Arrays {
        public static byte[] CopyOfRange(byte[] src, int start, int end) {
            var len = end - start;
            var dest = new byte[len];
            Array.Copy(src, start, dest, 0, len);
            return dest;
        }

        public static string ByteArrayToHex(byte[] ba) {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "").ToLower();
        }
    }

    public static class Bytes {
        /// <summary>
        /// Will correct a little endian byte[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] CheckEndian(byte[] bytes) {
            if (BitConverter.IsLittleEndian) {
                var byteList = bytes.Reverse(); // Must be in bigendian
                return byteList.ToArray();
            } else {
                return bytes;
            }
        }
    }

    public static class Times {
        public static byte[] MilliTime() {// TODO Make private
            List<byte> byteList = new List<byte>();
            DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // 6 Byte millisec unix time
            var unixMilliLong = (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
            byte[] unixBytes = Bytes.CheckEndian(BitConverter.GetBytes(unixMilliLong));
            unixBytes = Arrays.CopyOfRange(unixBytes, 2, unixBytes.Length);
            return unixBytes;
        }
    }

    public static class Entries {
        public static byte[] HashEntry(DataStructs.EntryData entry) { //TODO: Make private, public for tests
            byte[] data = MarshalBinary(entry); // TODO: Check for error
            byte[] h1 = SHA512.Create().ComputeHash(data);
            byte[] h2 = new byte[h1.Length + data.Length];
            h1.CopyTo(h2, 0);
            data.CopyTo(h2, h1.Length);
            byte[] h3 = SHA256.Create().ComputeHash(h2);
            return h3;
        }

        public static byte[] MarshalBinary(DataStructs.EntryData e) { //TODO: Make private
            List<byte> entryBStruct = new List<byte>();
            byte[] idsSize = MarshalExtIDsSize(e);


            idsSize = Bytes.CheckEndian(idsSize);
            // Header 
            // 1 byte version
            byte version = 0;
            entryBStruct.Add(version);
            // 32 byte chainid
            byte[] chain = Strings.DecodeHexIntoBytes(e.ChainID);
            entryBStruct.AddRange(chain);
            // Ext Ids Size
            entryBStruct.AddRange(idsSize);

            // Payload
            // ExtIDS
            if (e.ExtIDs != null) {
                byte[] ids = MarshalExtIDsBinary(e);
                entryBStruct.AddRange(ids);
            }
            // Content
            byte[] content = Encoding.ASCII.GetBytes(e.Content);
            entryBStruct.AddRange(content);

            return entryBStruct.ToArray();
        }

        private static byte[] MarshalExtIDsBinary(DataStructs.EntryData e) {
            List<byte> byteList = new List<byte>();
            foreach (var exID in e.ExtIDs) {
                // 2 byte size of ExtID
                Int16 extLen = Convert.ToInt16(exID.Length);
                byte[] bytes = BitConverter.GetBytes(extLen);
                bytes = Bytes.CheckEndian(bytes);
                byteList.AddRange(bytes);
                byte[] extIDStr = Encoding.ASCII.GetBytes(exID);
                byteList.AddRange(extIDStr);
            }
            return byteList.ToArray();
        }

        private static byte[] MarshalExtIDsSize(DataStructs.EntryData e) {
            if (e.ExtIDs == null) {
                Int16 extLen = 0;
                byte[] bytes = BitConverter.GetBytes(extLen);
                return Bytes.CheckEndian(bytes);
            } else {
                var totalSize = 0;
                foreach (var extElement in e.ExtIDs) {
                    totalSize += extElement.Length + 2;
                }

                Int16 extLen = Convert.ToInt16(totalSize);


                byte[] bytes = BitConverter.GetBytes(extLen);
                return bytes;
                // return Bytes.CheckEndian(bytes);
            }
        }

        public static sbyte EntryCost(DataStructs.EntryData entry) {
            byte[] entryBinary = Entries.MarshalBinary(entry);
            var len = entryBinary.Length - 35;
            if (len > 10240) {
                return 10; //Error, cannot be larger than 10kb
            }
            var r = len % 1024;
            sbyte n = (sbyte)(len / 1024); // Capacity of Entry Payment

            if (r > 0) {
                n += 1;
            }
            if (n < 1) {
                n = 1;
            }
            return n;
        }
    }

    public static class Strings {
        public static byte[] DecodeHexIntoBytes(string input) {
            var result = new byte[(input.Length + 1) >> 1];
            int lastcell = result.Length - 1;
            int lastchar = input.Length - 1;
            // count up in characters, but inside the loop will
            // reference from the end of the input/output.
            for (int i = 0; i < input.Length; i++) {
                // i >> 1    -  (i / 2) gives the result byte offset from the end
                // i & 1     -  1 if it is high-nibble, 0 for low-nibble.
                result[lastcell - (i >> 1)] |= ByteLookup[i & 1, HexToInt(input[lastchar - i])];
            }
            return result;
        }


        private static int HexToInt(char c) {
            switch (c) {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                case 'A':
                    return 10;
                case 'b':
                case 'B':
                    return 11;
                case 'c':
                case 'C':
                    return 12;
                case 'd':
                case 'D':
                    return 13;
                case 'e':
                case 'E':
                    return 14;
                case 'f':
                case 'F':
                    return 15;
                default:
                    throw new FormatException("Unrecognized hex char " + c);
            }
        }
        private static readonly byte[,] ByteLookup = new byte[,]
        {
            // low nibble
            {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f},
            // high nibble
            {0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0}
        };
    }

    public static class ControlExtensions {
        public static void SetPropertyThreadSafe<TResult>(
            this Control @this,
            Expression<Func<TResult>> property,
            TResult value) {
            var propertyInfo = (property.Body as MemberExpression).Member
                as PropertyInfo;

            if (propertyInfo == null ||
                !@this.GetType().IsSubclassOf(propertyInfo.ReflectedType) ||
                @this.GetType().GetProperty(
                    propertyInfo.Name,
                    propertyInfo.PropertyType) == null) {
                throw new ArgumentException(
                    "The lambda expression 'property' must reference a valid property on this Control.");
            }

            if (@this.InvokeRequired) {
                @this.Invoke(new SetPropertyThreadSafeDelegate<TResult>
                    (SetPropertyThreadSafe), @this, property, value);
            }
            else {
                @this.GetType().InvokeMember(
                    propertyInfo.Name,
                    BindingFlags.SetProperty,
                    null,
                    @this,
                    new object[] {value});
            }
        }

        private delegate void SetPropertyThreadSafeDelegate<TResult>(
            Control @this,
            Expression<Func<TResult>> property,
            TResult value);
    }
}