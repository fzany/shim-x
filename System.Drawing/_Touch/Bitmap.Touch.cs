//
// AForge Image Processing Library
// Portable AForge.NET framework
// https://github.com/cureos/aforge
//
// Shim.System.Drawing
//
// Copyright © Cureos AB, 2013-2014
// info at cureos dot com
//

using System.Drawing.Imaging;
using System.IO;
using MonoTouch.CoreGraphics;

namespace System.Drawing
{
	// TODO Implement methods and operators
	public sealed partial class Bitmap
	{
		#region METHODS

		public static Bitmap FromStream(Stream stream)
		{
			throw new NotImplementedException();
		}

		private static PixelFormat GetPixelFormat(CGColorSpace colorSpace, CGBitmapFlags bitmapInfo)
		{
			return PixelFormat.Format32bppArgb;
		}

		#endregion

		#region OPERATORS

		public static explicit operator CGImage(Bitmap bitmap)
		{
			var bytesPerRow = bitmap._stride;

			CGColorSpace colorSpace;
			CGImageAlphaInfo bitmapInfo;
			switch (bitmap._pixelFormat)
			{
				case PixelFormat.Format32bppPArgb:
					colorSpace = CGColorSpace.CreateDeviceRGB();
					bitmapInfo = CGImageAlphaInfo.PremultipliedLast;
					break;
				case PixelFormat.Format8bppIndexed:
					colorSpace = CGColorSpace.CreateDeviceGray();
					bitmapInfo = CGImageAlphaInfo.None;
					break;
				default:
					throw new InvalidOperationException();
			}
			using (
				var context = new CGBitmapContext(bitmap._scan0, bitmap._width, bitmap._height, 8, bytesPerRow, colorSpace,
					bitmapInfo))
			{
				return context.ToImage();
			}
		}

		public static explicit operator Bitmap(CGImage cgImage)
		{
			var width = cgImage.Width;
			var pixelFormat = GetPixelFormat(cgImage.ColorSpace, cgImage.BitmapInfo);
			return new Bitmap(width, cgImage.Height, GetStride(width, pixelFormat), pixelFormat,
				cgImage.DataProvider.CopyData().Bytes);
		}

		#endregion
	}
}
