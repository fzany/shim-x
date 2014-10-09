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
