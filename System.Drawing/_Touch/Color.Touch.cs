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

using System.Linq;
using MonoTouch.CoreGraphics;

namespace System.Drawing
{
	public partial struct Color
	{
		#region OPERATORS

		public static implicit operator CGColor(Color color)
		{
			return new CGColor(color._r / 255.0f, color._g / 255.0f, color._b / 255.0f, color._a / 255.0f);
		}

		public static implicit operator Color(CGColor cgColor)
		{
			var components = cgColor.Components.Select(component => (int)(255.0f * component)).ToList();
			switch (cgColor.NumberOfComponents)
			{
				case 2:
					return FromArgb(components[1], components[0], components[0], components[0]);
				case 4:
					return FromArgb(components[3], components[0], components[1], components[2]);
				default:
					throw new ArgumentException("Invalid number of color components", "cgColor");
			}
		}

		#endregion
	}
}