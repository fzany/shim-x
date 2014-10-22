// Copyright (c) 2010-2014 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

namespace HelloAForge
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;

    using Accord.Imaging.Filters;
    using Accord.Vision.Detection;

    using Xamarin.Forms;

    using Color = System.Drawing.Color;
    using Image = System.Drawing.Image;

    public partial class MainPage
    {
        #region FIELDS

        private Stream imageSourceStream;
        private readonly Bitmap bitmap;
        private readonly HaarObjectDetector detector;

        #endregion

        #region CONSTRUCTORS

        public MainPage()
        {
            InitializeComponent();

            var assembly = ShimAssembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("HelloAForge.Images.judybats.jpg"))
            {
                this.imageView.Source = this.GetImageSourceFromStream(stream);

                stream.Seek(0, SeekOrigin.Begin);
                this.bitmap = ((Bitmap)Image.FromStream(stream)).Clone(PixelFormat.Format32bppArgb);
            }

            foreach (var searchMode in Enum.GetNames(typeof(ObjectDetectorSearchMode)))
            {
                this.searchModePicker.Items.Add(searchMode);
            }
            foreach (var scalingMode in Enum.GetNames(typeof(ObjectDetectorScalingMode)))
            {
                this.scalingModePicker.Items.Add(scalingMode);
            }

            this.searchModePicker.SelectedIndex =
                this.searchModePicker.Items.IndexOf(ObjectDetectorSearchMode.NoOverlap.ToString());
            this.scalingModePicker.SelectedIndex =
                this.scalingModePicker.Items.IndexOf(ObjectDetectorScalingMode.SmallerToGreater.ToString());

            using (var stream = assembly.GetManifestResourceStream("HelloAForge.Files.haarcascade_frontalface_alt.xml"))
            {
                var cascade = HaarCascade.FromXml(stream);
                this.detector = new HaarObjectDetector(cascade, 30);
            }
        }

        #endregion

        #region EVENT HANDLERS

        private void OnDetectButtonClicked(object sender, EventArgs args)
        {
            int searchModeIndex, scalingModeIndex;
            ObjectDetectorSearchMode searchMode;
            ObjectDetectorScalingMode scalingMode;
            if ((searchModeIndex = this.searchModePicker.SelectedIndex) < 0
                || (scalingModeIndex = this.scalingModePicker.SelectedIndex) < 0
                || !Enum.TryParse(this.searchModePicker.Items[searchModeIndex], out searchMode)
                || !Enum.TryParse(this.scalingModePicker.Items[scalingModeIndex], out scalingMode))
            {
                return;
            }

            this.detector.SearchMode = searchMode;
            this.detector.ScalingMode = scalingMode;
            this.detector.ScalingFactor = 1.5f;
            this.detector.UseParallelProcessing = false;

            // Process frame to detect objects
            var objects = this.detector.ProcessFrame(this.bitmap);

            if (objects.Length > 0)
            {
                var marker = new RectanglesMarker(objects, Color.FromArgb(0xff, 0xff, 0x00, 0xff));

                var stream = new MemoryStream();
                marker.Apply(this.bitmap).Save(stream, ImageFormat.Jpeg);

                this.imageView.Source = this.GetImageSourceFromStream(stream);
            }
        }

        #endregion

        #region METHODS

        private ImageSource GetImageSourceFromStream(Stream stream)
        {
            this.imageSourceStream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(this.imageSourceStream);

            return ImageSource.FromStream(
                () =>
                    {
                        this.imageSourceStream.Seek(0, SeekOrigin.Begin);
                        return this.imageSourceStream;
                    });
        }

        #endregion
    }
}
