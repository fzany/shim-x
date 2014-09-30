using MonoTouch.UIKit;
using System.Drawing;

namespace HelloAForgeX
{
    using MonoTouch.CoreGraphics;

    public class MyViewController : UIViewController
	{
		private UIImage image;

		UIButton button;
		int numClicks = 0;
		float buttonWidth = 200;
		float buttonHeight = 50;

		public MyViewController()
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.Frame = UIScreen.MainScreen.Bounds;
			View.BackgroundColor = UIColor.White;
			View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

/*			button = UIButton.FromType(UIButtonType.RoundedRect);

			button.Frame = new RectangleF(
				View.Frame.Width / 2 - buttonWidth / 2,
				View.Frame.Height / 2 - buttonHeight / 2,
				buttonWidth,
				buttonHeight);

			button.SetTitle("Click me", UIControlState.Normal);

			button.TouchUpInside += (object sender, EventArgs e) =>
			{
				button.SetTitle(String.Format("clicked {0} times", numClicks++), UIControlState.Normal);
			};

			button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin |
				UIViewAutoresizing.FlexibleBottomMargin;

			View.AddSubview(button);
 */
			image = UIImage.FromResource(null, "HelloAForgeX.Images.coins.jpg");

			Bitmap bitmap = (Bitmap)image.CGImage;
			var filter1 = AForge.Imaging.Filters.Grayscale.CommonAlgorithms.RMY;
			bitmap = filter1.Apply(bitmap);
			var filter2 = new AForge.Imaging.Filters.CannyEdgeDetector();
			filter2.ApplyInPlace(bitmap);
//			bitmap = AForge.Imaging.Image.Clone(bitmap, PixelFormat.Format32bppArgb);

			var imageView = new UIImageView(new UIImage((CGImage)bitmap)) { Frame = View.Frame };
			View.AddSubview(imageView);
		}

	}
}

