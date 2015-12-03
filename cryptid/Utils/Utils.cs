#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

#endregion

namespace Cryptid.Utils {
    public static class Keys {
        /// <summary>
        ///     Gets an RSA private key at the specified path
        /// </summary>
        /// <param name="path">The path to the private key</param>
        /// <returns>The private key RSA parameters</returns>
        public static RSAParameters PrivateKey(string path) {
            var k = new RSACryptoServiceProvider();
            k.FromXmlString(File.ReadAllText(path));
            return k.ExportParameters(true);
        }

        /// <summary>
        ///     Gets an RSA public key at the specified path
        /// </summary>
        /// <param name="path">The path to the public key</param>
        /// <returns>The public key RSA parameters</returns>
        public static RSAParameters PublicKey(string path) {
            var k = new RSACryptoServiceProvider();
            k.FromXmlString(File.ReadAllText(path));
            return k.ExportParameters(false);
        }
    }

    /// <summary>
    ///     Provides a way to easily assign a string
    ///     to an enum.
    /// </summary>
    public class StringValue : Attribute {
        public StringValue(string value) {
            Value = value;
        }

        public string Value { get; }
    }


    public static class Arrays {
        /// <summary>
        ///     Copys a section of an enumerable into another array
        /// </summary>
        /// <typeparam name="T">The enumerable's type</typeparam>
        /// <param name="source">The source enumerable</param>
        /// <param name="index">The start index of the slice</param>
        /// <param name="length">The length of the slice</param>
        /// <param name="padToLength">Should we pad the slice to the length specified</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Slices an enumerable into the specified number of parts
        /// </summary>
        /// <typeparam name="T">The enumerable's type</typeparam>
        /// <param name="source">The enumerable to cut</param>
        /// <param name="count">The number of slices</param>
        /// <param name="padToLength">Whether or not to pad the slices to an equal length</param>
        /// <returns></returns>
        public static IEnumerable<T[]> Slices<T>(this T[] source, int count, bool padToLength = false) {
            for (var i = 0; i < source.Length; i += count)
                yield return source.CopySlice(i, count, padToLength);
        }

        /// <summary>
        ///     Convience function to emulate Java's CopyOfRange
        /// </summary>
        /// <param name="src">The byte array to copfrom</param>
        /// <param name="start">The index to cut from</param>
        /// <param name="end">The index to cut to</param>
        /// <returns></returns>
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
        ///     Checks if two byte arrays are equal
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

        /// <summary>
        ///     Checks if a byte sequence begins with another byte sequence
        /// </summary>
        /// <param name="haystack">The sequence to check</param>
        /// <param name="needle">The sequence we want to find in the haystack</param>
        /// <returns>Whether or not needle was found in haystack</returns>
        public static bool StartsWith(byte[] haystack, byte[] needle) {
            if (needle.Length > haystack.Length) return false;
            for (var i = 0; i < needle.Length; i++) {
                if (haystack[i] != needle[i]) return false;
            }
            return true;
        }
    }

    public static class Strings {
        private static readonly byte[,] ByteLookup = {
            // low nibble
            {0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f},
            // high nibble
            {0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0}
        };

        /// <summary>
        ///     Gets a random string of a specified length
        /// </summary>
        /// <param name="length">The length of the string</param>
        /// <returns>The random string</returns>
        public static string RandomString(int length) {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        ///     Converts string hex into byte[]
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
        ///     If hex string has "-", this method removes them
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveDashes(string s) {
            return s.Replace("-", "");
        }

        /// <summary>
        ///     Helper function of Hex functions
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
        /// <summary>
        ///     Extension method to set a property in a threadsafe manor
        /// </summary>
        /// <typeparam name="TResult">The thread result</typeparam>
        /// <param name="@this">The control to set the property on</param>
        /// <param name="property">The property to change</param>
        /// <param name="value">The new value of the property</param>
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