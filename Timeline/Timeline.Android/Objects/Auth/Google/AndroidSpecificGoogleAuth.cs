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
using Timeline.Objects.Auth.Google;
using Plugin.CurrentActivity;
using Xamarin.Forms;

[assembly: Dependency(typeof(Timeline.Droid.Objects.Auth.Google.AndroidSpecificGoogleAuth))]
namespace Timeline.Droid.Objects.Auth.Google
{
    class AndroidSpecificGoogleAuth : IPlatformSpecificGoogleAuth
    {
        // Need to be static because we need to access it in GoogleAuthInterceptor for continuation
        public static GoogleAuthenticator staticAuth;

        public string PlatformClientID { get; private set; }

        public AndroidSpecificGoogleAuth()
        {
            //android client id:
            PlatformClientID = "597410249897-e5iqvca2pnvg6hlm4i5t4h5tgk8liutk.apps.googleusercontent.com";
        }

        public void AuthenticateGoogle(GoogleAuthenticator googleAuthenticator)
        {
            staticAuth = googleAuthenticator; //saving it as static, so the interceptor can call it
            var authenticator = googleAuthenticator.GetAuthenticator();
            var intent = authenticator.GetUI(CrossCurrentActivity.Current.AppContext);
            CrossCurrentActivity.Current.AppContext.StartActivity(intent);
        }

        public void ClearStaticAuth()
        {
            staticAuth = null;
        }
    }
}