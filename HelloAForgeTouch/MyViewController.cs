// Copyright (c) 2010-2014 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.
namespace HelloAForgeTouch
{
    using System.Drawing;
	using System.Drawing.Imaging;
	using System.IO;

    using MonoTouch.CoreGraphics;
    using MonoTouch.UIKit;

	using AForge.Imaging.Filters;

    public class MyViewController : UIViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.View.Frame = UIScreen.MainScreen.Bounds;
            this.View.BackgroundColor = UIColor.White;
            this.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            var image = UIImage.FromResource(null, "HelloAForgeTouch.Images.coins.jpg");

            var bitmap = (Bitmap)image.CGImage;
			var filter = new FiltersSequence (Grayscale.CommonAlgorithms.RMY, new CannyEdgeDetector ());
            bitmap = filter.Apply(bitmap);

			using (var stream = new MemoryStream())
			{
				bitmap.Save(stream, ImageFormat.Png);
				stream.Seek(0, SeekOrigin.Begin);
				bitmap = (Bitmap)Image.FromStream(stream);
			}

            var imageView = new UIImageView(new UIImage((CGImage)bitmap)) { Frame = this.View.Frame };
            this.View.AddSubview(imageView);
        }

    }
}

