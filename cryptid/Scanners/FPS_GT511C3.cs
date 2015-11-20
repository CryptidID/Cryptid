#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace Cryptid.Scanners {
    // ReSharper disable once InconsistentNaming
    public static unsafe class FPS_GT511C3 {
        private static Bitmap GetImageFromBytes(byte[] data, int width, int height) {
            var b = new Bitmap(width, height);

            var i = 0;
            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    b.SetPixel(x, y, Color.FromArgb(data[i], data[i], data[i]));
                    i++;
                }
            }

            return b;
        }

        public static Bitmap GetRawImage() {
            var ret = new byte[240*216];
            var outBuf = NativeMethods.get_raw_image();
            for (var i = 0; i < ret.Length; i++) ret[i] = outBuf[i];

            return GetImageFromBytes(ret, 240, 216);
        }

        public static Bitmap GetImage() {
            var ret = new byte[240*216];
            var outBuf = NativeMethods.get_image();
            for (var i = 0; i < ret.Length; i++) ret[i] = outBuf[i];

            return GetImageFromBytes(ret, 240, 216);
        }

        public static int IsPressingFinger() {
            return NativeMethods.is_press_finger();
        }

        public static int SetCmosLed(bool b) {
            return NativeMethods.cmos_led(b);
        }

        public static int Open(int port, int baud) {
            try {
                return NativeMethods.open(new IntPtr(port), new IntPtr(baud));
            }
                // ReSharper disable once UnusedVariable
            catch (AccessViolationException ave) {
                Console.WriteLine("GOT ACCESS VIOLATION EXCEPTION ON OPEN! POSSIBLE MEMORY HACK ATTEMPT.");
                return -15;
            }
        }

        public static int Close() {
            return NativeMethods.close();
        }

        public static class NativeMethods {
            [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern byte* get_raw_image();

            [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern byte* get_image();

            [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int is_press_finger();

            [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int close();

            [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int open(IntPtr port, IntPtr baud);

            [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int cmos_led([MarshalAs(UnmanagedType.Bool)] bool b);
        }
    }
}