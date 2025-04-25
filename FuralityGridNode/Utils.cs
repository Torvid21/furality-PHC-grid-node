using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace FuralityGridNode
{
    static class Utils
    {
        // Read png into RGBA byte array
        public static (byte[] data, int sizeX, int sizeY) ReadPng(string path)
        {
            var bmp = new Bitmap(path);                       // System.Drawing loads any PNG depth, we’ll force 32-bit below
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            // Lock the pixels as 32-bit ARGB (in memory the order is B-G-R-A)
            var bits = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            try
            {
                int byteCount = bits.Stride * bits.Height;
                byte[] argb = new byte[byteCount];
                Marshal.Copy(bits.Scan0, argb, 0, byteCount);

                int width = bmp.Width;
                int height = bmp.Height;
                byte[] rgba = new byte[bmp.Width * bmp.Height * 4];

                for (int y = 0; y < bmp.Height; y++)
                {
                    int rowSrc = y * bits.Stride;
                    int rowDst = y * bmp.Width * 4;

                    for (int x = 0; x < bmp.Width; x++)
                    {
                        int iSrc = rowSrc + x * 4;   // B-G-R-A
                        int iDst = rowDst + x * 4;   // R-G-B-A

                        // Swap channels B<->R
                        rgba[iDst] = argb[iSrc + 2];   // R
                        rgba[iDst + 1] = argb[iSrc + 1];   // G
                        rgba[iDst + 2] = argb[iSrc];   // B
                        rgba[iDst + 3] = argb[iSrc + 3];   // A
                    }
                }

                bmp.UnlockBits(bits);
                bmp.Dispose();
                return (rgba, width, height);
            }
            catch
            {
                return (null, 0, 0);
            }
        }

        // Write png from RGBA byte array
        public static void WritePng(string path, byte[] data, int sizeX, int sizeY)
        {
            if (data.Length != sizeX * sizeY * 4)
                throw new ArgumentException("data length does not match dimensions");

            var bmp = new Bitmap(sizeX, sizeY, PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, sizeX, sizeY);
            var bits = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            try
            {
                int byteCount = bits.Stride * bits.Height;
                byte[] argb = new byte[byteCount];

                for (int y = 0; y < sizeY; y++)
                {
                    int rowDst = y * bits.Stride;
                    int rowSrc = y * sizeX * 4;

                    for (int x = 0; x < sizeX; x++)
                    {
                        int iSrc = rowSrc + x * 4;   // R-G-B-A
                        int iDst = rowDst + x * 4;   // B-G-R-A

                        argb[iDst] = data[iSrc + 2];  // B
                        argb[iDst + 1] = data[iSrc + 1];  // G
                        argb[iDst + 2] = data[iSrc];  // R
                        argb[iDst + 3] = data[iSrc + 3];  // A
                    }
                }

                Marshal.Copy(argb, 0, bits.Scan0, byteCount);
            }
            finally
            {
                bmp.UnlockBits(bits);
            }

            bmp.Save(path, ImageFormat.Png);
            bmp.Dispose();
        }
    }
}
