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

namespace System.Drawing
{
    using System.Drawing.Imaging;
    using System.IO;

    using MonoTouch.CoreGraphics;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    public sealed partial class Bitmap
    {
        #region METHODS

        internal static Bitmap Create(Stream stream)
        {
            return (Bitmap)UIImage.LoadFromData(NSData.FromStream(stream)).CGImage;
        }

        internal void WriteTo(Stream stream, ImageFormat format)
        {
            var uiImage = new UIImage((CGImage)this);

            NSData compressedImage;
            if (format.Equals(ImageFormat.Jpeg))
            {
                compressedImage = uiImage.AsJPEG();
            }
            else if (format.Equals(ImageFormat.Png))
            {
                compressedImage = uiImage.AsPNG();
            }
            else
            {
                throw new ArgumentOutOfRangeException("format", format, "Only supported formats are JPEG and PNG");
            }

            compressedImage.AsStream().CopyTo(stream);
        }

        private static PixelFormat GetPixelFormat(CGImage cgImage)
        {
            var alphaInfo = cgImage.AlphaInfo;
            var bitsPerComponent = cgImage.BitsPerComponent;
            var components = cgImage.ColorSpace.Components;
            var model = cgImage.ColorSpace.Model;
            var unsupportedText =
                String.Format(
                    "Unsupported CGImage format. Model: {0}, Alpha: {1}, Components: {2}, Bits per component: {3}",
                    model,
                    alphaInfo,
                    components,
                    bitsPerComponent);

            switch (model)
            {
                case CGColorSpaceModel.RGB:
                    switch (components)
                    {
                        case 3:
                            switch (bitsPerComponent)
                            {
                                case 8:
                                    return PixelFormat.Format24bppRgb;  // Seems like 4 components are still sent, although one ignored?
                                case 16:
                                    return PixelFormat.Format48bppRgb;
                                default:
                                    throw new ArgumentException(unsupportedText);
                            }
                        case 4:
                            switch (bitsPerComponent)
                            {
                                case 8:
                                    switch (alphaInfo)
                                    {
                                        case CGImageAlphaInfo.First:
                                            return PixelFormat.Format32bppArgb;
                                        case CGImageAlphaInfo.PremultipliedFirst:
                                            return PixelFormat.Format32bppPArgb;
                                        case CGImageAlphaInfo.NoneSkipFirst:
                                            return PixelFormat.Format32bppRgb;
                                        default:
                                            throw new ArgumentException(unsupportedText);
                                    }
                                case 16:
                                    switch (alphaInfo)
                                    {
                                        case CGImageAlphaInfo.First:
                                            return PixelFormat.Format64bppArgb;
                                        case CGImageAlphaInfo.PremultipliedFirst:
                                            return PixelFormat.Format64bppPArgb;
                                        default:
                                            throw new ArgumentException(unsupportedText);
                                    }
                                default:
                                    throw new ArgumentException(unsupportedText);
                            }
                        default:
                            throw new ArgumentException(unsupportedText);
                    }
                case CGColorSpaceModel.Monochrome:
                    switch (bitsPerComponent)
                    {
                        case 16:
                            return PixelFormat.Format16bppGrayScale;
                        default:
                            throw new ArgumentException(unsupportedText);
                    }
                case CGColorSpaceModel.Indexed:
                    var baseComponents = cgImage.ColorSpace.GetBaseColorSpace().Components;
                    var indexCount = cgImage.ColorSpace.GetColorTable().Length / baseComponents;

                    if (indexCount <= 2) return PixelFormat.Format1bppIndexed;
                    if (indexCount <= 16) return PixelFormat.Format4bppIndexed;
                    if (indexCount <= 256) return PixelFormat.Format8bppIndexed;

                    throw new ArgumentException(unsupportedText);
                default:
                    throw new ArgumentException(unsupportedText);
            }
        }

        private static ColorPalette GetPaletteFromColorSpace(CGColorSpace colorSpace)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region OPERATORS

        public static explicit operator CGImage(Bitmap bitmap)
        {
            var bytesPerRow = bitmap._stride;

            int bitsPerComponent;
            CGColorSpace colorSpace;
            CGImageAlphaInfo bitmapInfo;

            switch (bitmap._pixelFormat)
            {
                case PixelFormat.Format32bppRgb:
                    bitsPerComponent = 8;
                    colorSpace = CGColorSpace.CreateDeviceRGB();
                    bitmapInfo = CGImageAlphaInfo.NoneSkipFirst;
                    break;
                case PixelFormat.Format32bppArgb:
                    bitsPerComponent = 8;
                    colorSpace = CGColorSpace.CreateDeviceRGB();
                    bitmapInfo = CGImageAlphaInfo.First;
                    break;
                case PixelFormat.Format32bppPArgb:
                    bitsPerComponent = 8;
                    colorSpace = CGColorSpace.CreateDeviceRGB();
                    bitmapInfo = CGImageAlphaInfo.PremultipliedFirst;
                    break;
                case PixelFormat.Format16bppGrayScale:
                    bitsPerComponent = 16;
                    colorSpace = CGColorSpace.CreateDeviceGray();
                    bitmapInfo = CGImageAlphaInfo.None;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            using (
                var context = new CGBitmapContext(
                    bitmap._scan0,
                    bitmap._width,
                    bitmap._height,
                    bitsPerComponent,
                    bytesPerRow,
                    colorSpace,
                    bitmapInfo))
            {
                return context.ToImage();
            }
        }

        public static explicit operator Bitmap(CGImage cgImage)
        {
            var width = cgImage.Width;
            var pixelFormat = GetPixelFormat(cgImage);

            var bitmap = new Bitmap(
                width,
                cgImage.Height,
                GetStride(width, pixelFormat),
                pixelFormat,
                cgImage.DataProvider.CopyData().Bytes);

            if (cgImage.ColorSpace.Model == CGColorSpaceModel.Indexed) bitmap.Palette = GetPaletteFromColorSpace(cgImage.ColorSpace);

            return bitmap;
        }

        #endregion
    }
}
