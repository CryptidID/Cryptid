#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace Cryptid.Scanners {
    /// <summary>
    ///     This class acts as a C# interface for the C++
    ///     driver for the FpsGt511C1R fingerprint scanner.
    /// </summary>
    public static unsafe class FpsGt511C1R {
        /// <summary>
        ///     Gets a Bitmap of a provided width and height from an array of bytes
        /// </summary>
        /// <param name="data">The byte array containing the bitmap data</param>
        /// <param name="width">The width of the output bitmap</param>
        /// <param name="height">The height of the output bitmap</param>
        /// <returns>The bitmap made from the bytes</returns>
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

        /// <summary>
        ///     Read a raw image from the fingerprint scanner
        /// </summary>
        /// <returns>The raw fingerprint image as a bitmap</returns>
        public static Bitmap GetRawImage() {
            var ret = new byte[240*216];
            var outBuf = NativeMethods.get_raw_image();
            for (var i = 0; i < ret.Length; i++) ret[i] = outBuf[i];

            return GetImageFromBytes(ret, 240, 216);
        }

        /// <summary>
        ///     Read an image from the fingerprint scanner
        /// </summary>
        /// <returns>The fingerprint image as a bitmap</returns>
        public static Bitmap GetImage() {
            var ret = new byte[240*216];
            var outBuf = NativeMethods.get_image();
            for (var i = 0; i < ret.Length; i++) ret[i] = outBuf[i];

            return GetImageFromBytes(ret, 240, 216);
        }

        /// <summary>
        ///     Checks whther or not the uset is pressing their finger on the
        ///     fingerprint scanner
        /// </summary>
        /// <returns>Whether or not they are pressing their finger</returns>
        public static bool IsPressingFinger() {
            return NativeMethods.is_press_finger() > 0;
        }

        /// <summary>
        ///     Set the fingerprint scanner LED to be on or off
        /// </summary>
        /// <param name="b">Whether or not the LED should be on</param>
        /// <returns>The status code of this operation</returns>
        public static int SetCmosLed(bool b) {
            return NativeMethods.cmos_led(b);
        }

        /// <summary>
        ///     Opens a stream to the fingerprint scanner on the specified port
        ///     with the specified baudrate
        /// </summary>
        /// <param name="port">The port to connect on</param>
        /// <param name="baud">The baudrate to use</param>
        /// <returns>The status code of this operation</returns>
        public static int Open(int port, int baud) {
            try {
                return NativeMethods.open(new IntPtr(port), new IntPtr(baud));
            }
            catch (AccessViolationException) {
                return -15;
            }
        }

        /// <summary>
        ///     Closes the stream to the fingerprint scanner
        /// </summary>
        /// <returns>The status code of this operation</returns>
        public static int Close() {
            return NativeMethods.close();
        }

        /// <summary>
        ///     The fingerprint scanner driver dll interface
        /// </summary>
        private static class NativeMethods {
            [DllImport(@"FpsGt511C1R.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern byte* get_raw_image();

            [DllImport(@"FpsGt511C1R.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern byte* get_image();

            [DllImport(@"FpsGt511C1R.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int is_press_finger();

            [DllImport(@"FpsGt511C1R.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int close();

            [DllImport(@"FpsGt511C1R.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int open(IntPtr port, IntPtr baud);

            [DllImport(@"FpsGt511C1R.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int cmos_led([MarshalAs(UnmanagedType.Bool)] bool b);
        }
    }
}