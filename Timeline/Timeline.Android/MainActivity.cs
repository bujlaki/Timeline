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

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            Window.SetStatusBarColor(Android.Graphics.Color.Argb(255, 35, 75, 100));
        }

    }
}

