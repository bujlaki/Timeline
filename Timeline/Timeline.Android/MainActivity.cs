using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Auth;
using Plugin.CurrentActivity;
using Acr.UserDialogs;
using Android.Gms.Ads;

namespace Timeline.Droid
{
    [Activity(Label = "Timeline", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            //don't show warning message when closing account selection page
            CustomTabsConfiguration.CustomTabsClosingMessage = null;

            //initialize CurrentActivity plugin
            CrossCurrentActivity.Current.Init(this, bundle);

            //initialize UserDialogs
            UserDialogs.Init(this);

            //initialize Ads
            MobileAds.Initialize(ApplicationContext, "ca-app-pub-5812987721297534~6792480507");

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            Window.SetStatusBarColor(Android.Graphics.Color.Argb(255, 35, 75, 100));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

