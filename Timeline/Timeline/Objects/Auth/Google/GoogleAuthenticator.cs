using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Timeline.Models;
using Xamarin.Auth;

namespace Timeline.Objects.Auth.Google
{
    public class GoogleAuthenticator
    {
        private string current_clientId;
        private string current_scope;
        private string current_RedirectUrl;
        private const string AuthorizeUrl = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        private const bool IsUsingNativeUI = true;

        private OAuth2Authenticator auth;
        private IGoogleAuthenticationDelegate authDelegate;

        public GoogleAuthenticator(string clientId, string scope, string redirectUrl, IGoogleAuthenticationDelegate authenticationDelegate)
        {
            authDelegate = authenticationDelegate;
            current_clientId = clientId;
            current_scope = scope;
            current_RedirectUrl = redirectUrl;

            auth = new OAuth2Authenticator(clientId, string.Empty, scope,
                                            new Uri(AuthorizeUrl),
                                            new Uri(redirectUrl),
                                            new Uri(AccessTokenUrl),
                                            null, IsUsingNativeUI);

            auth.Completed += OnAuthenticationCompleted;
            auth.Error += OnAuthenticationFailed;
        }

        public void ReInit()
        {
            auth.Completed -= OnAuthenticationCompleted;
            auth.Error -= OnAuthenticationFailed;

            auth = new OAuth2Authenticator(current_clientId, string.Empty, current_scope,
                                new Uri(AuthorizeUrl),
                                new Uri(current_RedirectUrl),
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
                //save the account
                AccountStore.Create().Save(e.Account, "Google");
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

        private async Task ValidateGoogleAccessToken(Account account)
        {
            //            $access_token = google_get_user_token($user_id); // get existing token from DB
            //$redirecturl = $Google_Permissions->redirecturl;
            //$client_id = $Google_Permissions->client_id;
            //$client_secret = $Google_Permissions->client_secret;
            //$redirect_uri = $Google_Permissions->redirect_uri;
            //$max_results = $Google_Permissions->max_results;

            //$url = 'https://www.googleapis.com/oauth2/v1/tokeninfo?access_token='.$access_token;
            //$response_contacts = curl_get_responce_contents($url);
            //$response = (json_decode($response_contacts));

            //        if (isset($response->issued_to))
            //        {
            //            return true;
            //        }
            //        else if (isset($response->error))
            //        {
            //            return false;
            //        }

            var req = new OAuth2Request("GET", new Uri("https://www.googleapis.com/oauth2/v1/tokeninfo"), null, account);
            var resp = await req.GetResponseAsync();
            var obj = JObject.Parse(resp.GetResponseText());
        }

        public async Task RefreshGoogleAccessToken(Account account)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> { { "refresh_token", account.Properties["refresh_token"] }, { "client_id", auth.ClientId }, { "grant_type", "refresh_token" } };
            var request = new OAuth2Request("POST", new Uri(AccessTokenUrl), dictionary, account);
            var response = await request.GetResponseAsync();
        }

        public async Task<GoogleUserInfo> GetGoogleUserInfo(Account account)
        {
            //get GOOGLE userinfo
            var req = new OAuth2Request("GET", new Uri("https://www.googleapis.com/oauth2/v2/userinfo"), null, account);
            var resp = await req.GetResponseAsync();
            var obj = JObject.Parse(resp.GetResponseText());

            GoogleUserInfo userInfo = new GoogleUserInfo();
            userInfo.UserId = obj["id"].ToString().Replace("\"", "");
            userInfo.UserName = obj["given_name"].ToString().Replace("\"", "");
            userInfo.Email = obj["email"].ToString().Replace("\"", "");
            userInfo.Picture = obj["picture"].ToString().Replace("\"", "");

            return userInfo;
        }
    }
}
