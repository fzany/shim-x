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

    public class MyViewController : UIViewController
    {
        private UIImage image;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.View.Frame = UIScreen.MainScreen.Bounds;
            this.View.BackgroundColor = UIColor.White;
            this.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            this.image = UIImage.FromResource(null, "HelloAForgeTouch.Images.coins.jpg");

            var bitmap = (Bitmap)this.image.CGImage;
            var filter1 = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.RMY;
            bitmap = filter1.Apply(bitmap);
            var filter2 = new AForge.Imaging.Filters.CannyEdgeDetector();
            filter2.ApplyInPlace(bitmap);
			bitmap = bitmap.Clone (PixelFormat.Format32bppPArgb);

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

