using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Xamarin.Auth;

namespace Timeline.Objects.Auth.Google
{
    public class GoogleAuthenticator
    {
        private const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        private const bool IsUsingNativeUI = true;

        private OAuth2Authenticator auth;
        private IGoogleAuthenticationDelegate authDelegate;

        public GoogleAuthenticator(string clientId, string scope, string redirectUrl, IGoogleAuthenticationDelegate authenticationDelegate)
        {
            authDelegate = authenticationDelegate;

            auth = new OAuth2Authenticator(clientId, string.Empty, scope,
                                            new Uri(AuthorizeUrl),
                                            new Uri(redirectUrl),
                                            new Uri(AccessTokenUrl),
                                            null, IsUsingNativeUI);

            auth.Completed += OnAuthenticationCompleted;
            auth.Error += OnAuthenticationFailed;
        }

        public OAuth2Authenticator GetAuthenticator()
        {
            return auth;
        }

        public void OnPageLoading(Uri uri)
        {
            auth.OnPageLoading(uri);
        }

        private void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                authDelegate.OnGoogleAuthCompleted(e.Account);
            }
            else
            {
                authDelegate.OnGoogleAuthCanceled();
            }
        }

        private void OnAuthenticationFailed(object sender, AuthenticatorErrorEventArgs e)
        {
            authDelegate.OnGoogleAuthFailed(e.Message, e.Exception);
        }
    }
}
