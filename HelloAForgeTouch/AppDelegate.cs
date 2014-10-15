// Copyright (c) 2010-2014 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

namespace HelloAForgeTouch
{
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

    [Register("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		MyViewController viewController;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			this.window = new UIWindow(UIScreen.MainScreen.Bounds);

			this.viewController = new MyViewController();
			this.window.RootViewController = this.viewController;

			this.window.MakeKeyAndVisible();

			return true;
		}
	}
}

