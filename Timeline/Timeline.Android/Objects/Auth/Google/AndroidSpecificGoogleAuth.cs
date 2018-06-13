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
        public static GoogleAuthenticator Auth;

        public void AuthenticateGoogle(IGoogleAuthenticationDelegate _delegate)
        {
            //android client id:
            string clientId = "597410249897-e5iqvca2pnvg6hlm4i5t4h5tgk8liutk.apps.googleusercontent.com";
            string scope = "email";
            string redirectUrl = "hu.iqtech.timeline:/oauth2redirect";
            Auth = new GoogleAuthenticator(clientId, scope, redirectUrl, _delegate);

            var authenticator = Auth.GetAuthenticator();
            var intent = authenticator.GetUI(CrossCurrentActivity.Current.AppContext);
            CrossCurrentActivity.Current.AppContext.StartActivity(intent);
        }
    }
}