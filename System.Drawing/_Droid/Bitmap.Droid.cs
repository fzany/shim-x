/*
 *  Copyright (c) 2013-2014, Cureos AB.
 *  All rights reserved.
 *  http://www.cureos.com
 *
 *	This file is part of Shim.NET.
 *
 *  Shim.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  Shim.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with Shim.NET.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    using System.Runtime.InteropServices;

    using Java.Nio;

    public sealed partial class Bitmap
    {
        #region METHODS

        public static Bitmap FromStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        private static PixelFormat GetPixelFormat(Android.Graphics.Format androidPixelFormat)
        {
            switch (androidPixelFormat)
            {
                // TODO Add other supported formats.
               case Android.Graphics.Format.A8:
                    return PixelFormat.Alpha;
                case Android.Graphics.Format.Rgb565:
                    return PixelFormat.Format16bppRgb565;
                case Android.Graphics.Format.Rgba8888:
                    return PixelFormat.Format32bppArgb;
                default:
                    throw new ArgumentOutOfRangeException("config", androidPixelFormat, "Unsupported Android Bitmap configuration.");
            }
        }

        #endregion

        #region OPERATORS

        public static explicit operator Android.Graphics.Bitmap(Bitmap bitmap)
        {
            var width = bitmap._width;
            var height = bitmap._height;
            var rowBytes = bitmap._stride;

            Android.Graphics.Bitmap.Config config;
            switch (bitmap._pixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    config = Android.Graphics.Bitmap.Config.Argb8888;
                    break;
                case PixelFormat.Format16bppRgb565:
                    config = Android.Graphics.Bitmap.Config.Rgb565;
                    break;
                case PixelFormat.Alpha:
                    config = Android.Graphics.Bitmap.Config.Alpha8;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var androidBitmap = Android.Graphics.Bitmap.CreateBitmap(width, height, config);

            var bytes = new byte[height * rowBytes];
            Marshal.Copy(bitmap._scan0, bytes, 0, height * rowBytes);

            using (var buffer = ByteBuffer.Allocate(bytes.Length))
            {
                buffer.Put(bytes);
                buffer.Rewind();
                androidBitmap.CopyPixelsFromBuffer(buffer);
            }
            
            return androidBitmap;
        }

        public static explicit operator Bitmap(Android.Graphics.Bitmap androidBitmap)
        {
            var width = androidBitmap.Width;
            var height = androidBitmap.Height;
            var rowBytes = androidBitmap.RowBytes;

            var pixelFormat = GetPixelFormat(androidBitmap.GetBitmapInfo().Format);

            var bytes = new byte[height * rowBytes];
            using (var buffer = ByteBuffer.Allocate(height * rowBytes))
            {
                androidBitmap.CopyPixelsToBuffer(buffer);
                buffer.Rewind();
                buffer.Get(bytes);
            }

            unsafe
            {
                fixed (byte* ptr = bytes)
                {
                    return new Bitmap(width, height, rowBytes, pixelFormat, (IntPtr)ptr);
                }
            }
        }

        #endregion
    }
}
