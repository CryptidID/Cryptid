using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Cryptid.Utils;
using Cryptid;
using Size = System.Drawing.Size;

namespace cryptid.Scanners {
    public static class FPS_GT511C3 {
        [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe byte* get_raw_image();

        [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe byte* get_image();

        [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int is_press_finger();

        [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int close();

        [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int open(IntPtr port, IntPtr baud);

        [DllImport(@"FPS_GT511C3.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int cmos_led([MarshalAs(UnmanagedType.Bool)] bool b);

        private static Bitmap GetImageFromBytes(byte[] data, int width, int height) {
            Bitmap b = new Bitmap(width, height);

            int i = 0;
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    b.SetPixel(x, y, Color.FromArgb(data[i], data[i], data[i]));
                    i++;
                }
            }

            return b;
        }

        public static unsafe Bitmap GetRawImage() {
             var ret = new byte[240 * 216];
             byte* outBuf = get_raw_image();
             for (int i = 0; i < ret.Length; i++) ret[i] = outBuf[i];

             return GetImageFromBytes(ret, 240, 216);
         }

        public static unsafe Bitmap GetImage() {
            var ret = new byte[240 * 216];
            byte* outBuf = get_image();
            for (int i = 0; i < ret.Length; i++) ret[i] = outBuf[i];

            return GetImageFromBytes(ret, 240, 216);
        }

        public static int IsPressingFinger() {
            return is_press_finger();
        }

        public static int SetCmosLed(bool b) {
            return cmos_led(b);
        }

        public static int Open(int port, int baud) {
            try {
                return open(new IntPtr(port), new IntPtr(baud));
            }
            catch (AccessViolationException ave) {
                Console.WriteLine("GOT ACCESS VIOLATION EXCEPTION ON OPEN! POSSIBLE MEMORY HACK ATTEMPT.");
                return -15;
            }
        }

        public static int Close() {
            return close();
        }
    }
}
