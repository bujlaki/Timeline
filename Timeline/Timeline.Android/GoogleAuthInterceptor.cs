using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timeline.Droid
{

    [Activity(Label = "GoogleAuthInterceptor", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    [
        IntentFilter 
        (
            actions: new[] { Intent.ActionView },
            Categories = new[] 
            {
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            },
            DataSchemes = new[] { "hu.iqtech.timeline" }, //CASE SENSITIVE
            DataPaths = new[] { "/oauth2redirect" }
        )
    ]
    public class GoogleAuthInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Android.Net.Uri uri_android = Intent.Data;
            Uri uri_netfx = new Uri(uri_android.ToString());
            
            Timeline.Droid.Objects.Auth.Google.AndroidSpecificGoogleAuth.staticAuth?.OnPageLoading(uri_netfx);

            var intent = new Intent(this, typeof(MainActivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            Finish();

            return;
        }
    }
}