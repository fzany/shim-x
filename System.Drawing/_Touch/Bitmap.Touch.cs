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
	using System.Runtime.InteropServices;

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

		private static Bitmap GetTouchAdaptedBitmap (Bitmap bitmap)
		{
			switch (bitmap._pixelFormat) {
			case PixelFormat.Format16bppGrayScale:
			case PixelFormat.Format16bppRgb555:
			case PixelFormat.Format16bppRgb565:
			case PixelFormat.Format24bppRgb:
			case PixelFormat.Format32bppRgb:
			case PixelFormat.Format48bppRgb:
				return bitmap.Clone (PixelFormat.Format32bppRgb);
			case PixelFormat.Format16bppArgb1555:
			case PixelFormat.Format32bppArgb:
			case PixelFormat.Format32bppPArgb:
			case PixelFormat.Format64bppArgb:
			case PixelFormat.Format64bppPArgb:
				return bitmap.Clone (PixelFormat.Format32bppPArgb);
			case PixelFormat.Format1bppIndexed:
				return bitmap.Clone (PixelFormat.Format32bppPArgb); // TODO Change to Format1bppIndexed
			case PixelFormat.Format4bppIndexed:
				return bitmap.Clone (PixelFormat.Format32bppPArgb);// TODO Change to Format4bppIndexed
			case PixelFormat.Format8bppIndexed:
			case PixelFormat.Indexed:
				return bitmap.Clone (PixelFormat.Format32bppPArgb);// TODO Change to Format8bppIndexed
			case PixelFormat.Alpha:
			case PixelFormat.PAlpha:
				return bitmap.Clone (PixelFormat.PAlpha);
			default:
				throw new ArgumentOutOfRangeException (
					"pixelFormat",
					bitmap._pixelFormat,
					"Unsupported pixel format for iOS");
			}
		}

		static CGColorSpace CreateColorSpace (ColorPalette palette)
		{
			throw new NotImplementedException ();
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
                                    return PixelFormat.Format32bppRgb;
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

        private static ColorPalette CreatePalette(CGColorSpace colorSpace)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region OPERATORS

        public static explicit operator CGImage(Bitmap bitmap)
        {
			var adaptedBitmap = GetTouchAdaptedBitmap (bitmap);

            int bitsPerComponent;
            CGColorSpace colorSpace;
            CGImageAlphaInfo alphaInfo;

			switch (adaptedBitmap._pixelFormat) {
			case PixelFormat.Format32bppRgb:
				bitsPerComponent = 8;
				colorSpace = CGColorSpace.CreateDeviceRGB ();
				alphaInfo = CGImageAlphaInfo.NoneSkipLast;
				break;
			case PixelFormat.Format32bppPArgb:
				bitsPerComponent = 8;
				colorSpace = CGColorSpace.CreateDeviceRGB ();
				alphaInfo = CGImageAlphaInfo.PremultipliedLast;
				break;
			case PixelFormat.Format64bppPArgb:
				bitsPerComponent = 16;
				colorSpace = CGColorSpace.CreateDeviceRGB ();
				alphaInfo = CGImageAlphaInfo.PremultipliedLast;
				break;
			case PixelFormat.Format1bppIndexed:
			case PixelFormat.Format4bppIndexed:
			case PixelFormat.Format8bppIndexed:
				// TODO CGBitmapContext does not support indexed color spaces; consider changing to CGImage constructor
				throw new NotImplementedException ("Support for indexed pixel formats not implemented");
			case PixelFormat.PAlpha:
				throw new NotImplementedException ("Support for alpha-only pixel formats not implemented");
			default:
				throw new InvalidOperationException ();
			}

            using (
                var context = new CGBitmapContext(
					adaptedBitmap._scan0,
					adaptedBitmap._width,
					adaptedBitmap._height,
                    bitsPerComponent,
					adaptedBitmap._stride,
                    colorSpace,
                    alphaInfo))
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

            if (cgImage.ColorSpace.Model == CGColorSpaceModel.Indexed) bitmap.Palette = CreatePalette(cgImage.ColorSpace);

            return bitmap;
        }

        #endregion
    }
}
