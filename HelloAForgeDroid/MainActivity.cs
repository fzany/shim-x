// Copyright (c) 2010-2014 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

using Android.App;
using Android.Widget;
using Android.OS;

using System.Drawing;

namespace HelloAForgeDroid
{
    using System.Drawing.Imaging;
    using System.IO;

    using AForge.Imaging.Filters;

    [Activity(Label = "HelloAForgeDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var view = FindViewById<ImageView>(Resource.Id.imageView);

            var androidBitmap = Android.Graphics.BitmapFactory.DecodeResource(Resources, Resource.Drawable.coins);

            var bitmap = (Bitmap)androidBitmap;
            bitmap = bitmap.Clone(PixelFormat.Format32bppArgb);

            var filter = new FiltersSequence(
                new IFilter[] { Grayscale.CommonAlgorithms.RMY, new SobelEdgeDetector() });
            bitmap = filter.Apply(bitmap);

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                bitmap = (Bitmap)Image.FromStream(stream);
            }

            view.SetImageBitmap((Android.Graphics.Bitmap)bitmap);
            
            var text = this.FindViewById<TextView>(Resource.Id.textView);
            text.Text = "Image is loaded";
        }
    }
}

