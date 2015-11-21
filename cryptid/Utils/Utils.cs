#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using Cryptid.Factom.API;

#endregion

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
        public static T[] CopySlice<T>(this T[] source, int index, int length, bool padToLength = false) {
            var n = length;
            T[] slice = null;

            if (source.Length < index + length) {
                n = source.Length - index;
                if (padToLength) {
                    slice = new T[length];
                }
            }

            if (slice == null) slice = new T[n];
            Array.Copy(source, index, slice, 0, n);
            return slice;
        }

        public static IEnumerable<T[]> Slices<T>(this T[] source, int count, bool padToLength = false) {
            for (var i = 0; i < source.Length; i += count)
                yield return source.CopySlice(i, count, padToLength);
        }

        public static byte[] CopyOfRange(byte[] src, int start, int end) {
            var len = end - start;
            var dest = new byte[len];
            Array.Copy(src, start, dest, 0, len);
            return dest;
        }

        /// <summary>
        ///     Converts byte[] to hex string
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static string ByteArrayToHex(byte[] ba) {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "").ToLower();
        }
    }

    public static class Bytes {
        /// <summary>
        ///     Will correct a little endian byte[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] CheckEndian(byte[] bytes) {
            if (BitConverter.IsLittleEndian) {
                var byteList = bytes.Reverse(); // Must be in bigendian
                return byteList.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Checks if two byte arrays are equal
        /// </summary>
        /// <param name="a1">Byte[] to be compared</param>
        /// <param name="b1">Byte[] to be compared</param>
        /// <returns>True if equal</returns>
        public static bool Equality(byte[] a1, byte[] b1) {
            if (a1.Length == b1.Length) {
                var i = 0;
                while (i < a1.Length && (a1[i] == b1[i])) //Earlier it was a1[i]!=b1[i]
                {
                    i++;
                }
                if (i == a1.Length) {
                    return true;
                }
            }
            return false;
        }

        public static bool StartsWith(byte[] haystack, byte[] needle) {
            if (needle.Length > haystack.Length) return false;
            for (var i = 0; i < needle.Length; i++) {
                if (haystack[i] != needle[i]) return false;
            }
            return true;
        }
    }

    public static class Times {
        public static byte[] MilliTime() {
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            // 6 Byte millisec unix time
            var unixMilliLong = (long) (DateTime.UtcNow - unixEpoch).TotalMilliseconds;
            var unixBytes = Bytes.CheckEndian(BitConverter.GetBytes(unixMilliLong));
            unixBytes = Arrays.CopyOfRange(unixBytes, 2, unixBytes.Length);
            return unixBytes;
        }
    }

    public static class Entries {
        /// <summary>
        /// Gets a hash of an entry
        /// </summary>
        /// <param name="entry">EntryData to be hashed</param>
        /// <returns>Hash of entry</returns>
        public static byte[] HashEntry(DataStructs.EntryData entry) {
            var data = MarshalBinary(entry);
            var h1 = SHA512.Create().ComputeHash(data);
            var h2 = new byte[h1.Length + data.Length];
            h1.CopyTo(h2, 0);
            data.CopyTo(h2, h1.Length);
            var h3 = SHA256.Create().ComputeHash(h2);
            return h3;
        }

        /// <summary>
        /// Passing the first entry of a Chain will get the chainId of that entry. Needs the ExtIDs to do this successfully
        /// </summary>
        /// <param name="entry">Entry object</param>
        /// <returns>ChainID</returns>
        public static byte[] ChainIdOfFirstEntry(DataStructs.EntryData entry) {
            var byteList = new List<byte>();
            foreach (var ext in entry.ExtIDs) {
                byteList.AddRange(SHA256.Create().ComputeHash(ext));
            }
            var b = byteList.ToArray();
            var chainInfo = SHA256.Create().ComputeHash(b);
            return chainInfo;
        }

        /// <summary>
        /// Marshals an entry into a byte[] to be sent to restAPI
        /// </summary>
        /// <param name="e">Entry to be marshaled</param>
        /// <returns>Marshaled entry</returns>
        public static byte[] MarshalBinary(DataStructs.EntryData e) {
            var entryBStruct = new List<byte>();
            var idsSize = MarshalExtIDsSize(e);


            idsSize = Bytes.CheckEndian(idsSize);
            // Header 
            // 1 byte version
            byte version = 0;
            entryBStruct.Add(version);
            // 32 byte chainid
            var chain = e.ChainId;
            entryBStruct.AddRange(chain);
            // Ext Ids Size
            entryBStruct.AddRange(idsSize);

            // Payload
            // ExtIDS
            if (e.ExtIDs != null) {
                var ids = MarshalExtIDsBinary(e);
                entryBStruct.AddRange(ids);
            }
            // Content
            var content = e.Content;
            entryBStruct.AddRange(content);

            return entryBStruct.ToArray();
        }

        /// <summary>
        /// Helper function of MarshalBinary
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static byte[] MarshalExtIDsBinary(DataStructs.EntryData e) {
            var byteList = new List<byte>();
            foreach (var exId in e.ExtIDs) {
                // 2 byte size of ExtID
                var extLen = Convert.ToInt16(exId.Length);
                var bytes = BitConverter.GetBytes(extLen);
                bytes = Bytes.CheckEndian(bytes);
                byteList.AddRange(bytes);
                var extIdStr = exId;
                byteList.AddRange(extIdStr);
            }
            return byteList.ToArray();
        }

        /// <summary>
        /// Helper function of MarshalBinary
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static byte[] MarshalExtIDsSize(DataStructs.EntryData e) {
            if (e.ExtIDs == null) {
                short extLen = 0;
                var bytes = BitConverter.GetBytes(extLen);
                return Bytes.CheckEndian(bytes);
            }
            else {
                var totalSize = 0;
                foreach (var extElement in e.ExtIDs) {
                    totalSize += extElement.Length + 2;
                }

                var extLen = Convert.ToInt16(totalSize);


                var bytes = BitConverter.GetBytes(extLen);
                return bytes;
                // return Bytes.CheckEndian(bytes);
            }
        }

        /// <summary>
        /// Caculates the cost of an entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static sbyte EntryCost(DataStructs.EntryData entry) {
            var entryBinary = MarshalBinary(entry);
            var len = entryBinary.Length - 35;
            if (len > 10240) {
                //Error, cannot be larger than 10kb
                throw new ArgumentException("Parameter cannot exceed 10kb of content", nameof(entry));
            }
            var r = len%1024;
            var n = (sbyte) (len/1024); // Capacity of Entry Payment

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
        private static readonly byte[,] ByteLookup = {
            // low nibble
            {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f},
            // high nibble
            {0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0}
        };

        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Converts string hex into byte[]
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] DecodeHexIntoBytes(string input) {
            var result = new byte[(input.Length + 1) >> 1];
            var lastcell = result.Length - 1;
            var lastchar = input.Length - 1;
            // count up in characters, but inside the loop will
            // reference from the end of the input/output.
            for (var i = 0; i < input.Length; i++) {
                // i >> 1    -  (i / 2) gives the result byte offset from the end
                // i & 1     -  1 if it is high-nibble, 0 for low-nibble.
                result[lastcell - (i >> 1)] |= ByteLookup[i & 1, HexToInt(input[lastchar - i])];
            }
            return result;
        }

        /// <summary>
        ///  If hex string has "-", this method removes them
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveDashes(string s) {
            return s.Replace("-", "");
        }

        /// <summary>
        /// Helper function of Hex functions
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
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
    }

    public static class ControlExtensions {
        public static void SetPropertyThreadSafe<TResult>(
            this Control @this,
            Expression<Func<TResult>> property,
            TResult value) {
            var propertyInfo = (property.Body as MemberExpression)?.Member
                as PropertyInfo;

            if (propertyInfo?.ReflectedType != null &&
                (propertyInfo == null || !@this.GetType().IsSubclassOf(propertyInfo.ReflectedType) ||
                 @this.GetType().GetProperty(propertyInfo.Name, propertyInfo.PropertyType) == null)) {
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