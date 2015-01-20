// Copyright (c) 2010-2015 Anders Gustafsson, Cureos AB.
// All rights reserved. Any unauthorised reproduction of this 
// material will constitute an infringement of copyright.

namespace HelloAForge.WinPhone
{
    using Microsoft.Phone.Controls;

    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            SupportedOrientations = SupportedPageOrientation.PortraitOrLandscape;

            Xamarin.Forms.Forms.Init();
            LoadApplication(new HelloAForge.App());
        }
    }
}
