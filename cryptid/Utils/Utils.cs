using System;
using System.Runtime.InteropServices;

namespace Cryptid.Utils {
    public class StringValue : Attribute {
        private readonly string _value;

        public StringValue(string value) {
            _value = value;
        }

        public string Value {
            get { return _value; }
        }
    }


    public static class Arrays {
        public static byte[] CopyOfRange(byte[] src, int start, int end) {
            int len = end - start;
            byte[] dest = new byte[len];
            Array.Copy(src, start, dest, 0, len);
            return dest;
        }
    }
}
