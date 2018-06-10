using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Amazon.CognitoIdentity;
using Amazon;
using Xamarin.Auth;
using System.Threading.Tasks;

namespace Timeline.Droid
{
    [Activity(Label = "Timeline", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        CognitoAWSCredentials credentials;
        bool bDidLogin;
        public static OAuth2Authenticator auth;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            credentials = new CognitoAWSCredentials(
                "eu-central-1:fd027885-da62-40c8-a16e-44c8a7cb8300", // Identity pool ID
                RegionEndpoint.EUCentral1 // Region
            );
            //don't show warning message when closing account selection page
            CustomTabsConfiguration.CustomTabsClosingMessage = null;

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            Login();
        }

        private void ShowMessage(string message)
        {
            AlertDialog dlgAlert = new AlertDialog.Builder(this).Create();
            dlgAlert.SetMessage(message);
            dlgAlert.SetButton("Close", (s, args) => { dlgAlert.Dismiss(); });
            dlgAlert.Show();
        }

        public void Logout()
        {
            credentials.Clear();
        }

        public void Login()
        {
            credentials.Clear();

            if (!string.IsNullOrEmpty(credentials.GetCachedIdentityId()) || credentials.CurrentLoginProviders.Length > 0)
            {
                if (!bDidLogin)
                    ShowMessage(string.Format("I still remember you're {0} ", credentials.GetIdentityId()));
                bDidLogin = true;
                return;
            }

            bDidLogin = true;

            //web client id:        597410249897-82nm1rhaci9n0cektva3inaqvf48mitv.apps.googleusercontent.com
            //android client id:    597410249897-e5iqvca2pnvg6hlm4i5t4h5tgk8liutk.apps.googleusercontent.com

            auth = new Xamarin.Auth.OAuth2Authenticator(
               "597410249897-e5iqvca2pnvg6hlm4i5t4h5tgk8liutk.apps.googleusercontent.com",
               string.Empty,
               "profile",
               new System.Uri("https://accounts.google.com/o/oauth2/v2/auth"),
               new System.Uri("hu.iqtech.timeline:/oauth2redirect"),
               new System.Uri("https://www.googleapis.com/oauth2/v4/token"),
               isUsingNativeUI: true);

            auth.Completed += Auth_Completed;
            auth.Error += Auth_Error;
            StartActivity(auth.GetUI(this));
        }

        private void Auth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            ShowMessage("ERROR: " + e.Message); 
        }

        private void Auth_Completed(object sender, Xamarin.Auth.AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                var http = new System.Net.Http.HttpClient();
                var idToken = e.Account.Properties["id_token"];

                credentials.AddLogin("accounts.google.com", idToken);
                AmazonCognitoIdentityClient cli = new AmazonCognitoIdentityClient(credentials, RegionEndpoint.EUCentral1);
                var req = new Amazon.CognitoIdentity.Model.GetIdRequest();
                req.Logins.Add("accounts.google.com", idToken);
                req.IdentityPoolId = "eu-central-1:fd027885-da62-40c8-a16e-44c8a7cb8300";
                
                cli.GetIdAsync(req).ContinueWith((task) =>
                {
                    if ((task.Status == TaskStatus.RanToCompletion) && (task.Result != null))
                        ShowMessage(string.Format("Identity {0} retrieved", task.Result.IdentityId));
                    else
                        ShowMessage(task.Exception.InnerException != null ? task.Exception.InnerException.Message : task.Exception.Message);
                });
            }
            else
                ShowMessage("Login cancelled");
        }
    }
}

