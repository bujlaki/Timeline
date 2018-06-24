using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Amazon.Extensions.CognitoAuthentication;

using Timeline.Models;
using Timeline.Objects.Auth.Cognito;
using Timeline.Objects.Auth.Google;
using Amazon.CognitoIdentityProvider.Model;
using Acr.UserDialogs;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Xamarin.Auth;
using Newtonsoft.Json.Linq;


namespace Timeline.Services
{
    class AuthenticationService : IAuthenticationService, IGoogleAuthenticationDelegate
    {
        private IPlatformSpecificGoogleAuth googleAuth;
        private CognitoAuthenticator cognitoAuth;
        private IAuthenticationDelegate authDelegate;

        public MUser CurrentUser { get; private set; }
        public bool EmailVerificationNeeded { get; private set; }

        public AuthenticationService()
        {
            CurrentUser = new MUser();
            EmailVerificationNeeded = false;

            cognitoAuth = new CognitoAuthenticator();
            googleAuth = DependencyService.Get<IPlatformSpecificGoogleAuth>();
        }

        public async Task<bool> GetCachedCredentials()
        {
            try
            {
                IEnumerable<Account> accounts = AccountStore.Create().FindAccountsForService("Google");
                Account account = accounts.FirstOrDefault();
                if (account == null) return false;

                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    await GetGoogleUserinfo(account);
                    await GetAWSCredentialsForGoogleToken(account);
                }

                return true;
            }
            catch (Exception ex)
            {
                CurrentUser.Clear();
                return false;
            }
        }

        public async Task LoginCognito(string username, string password)
        {
            CurrentUser = await cognitoAuth.ValidateUser(username, password);
        }

        public async Task SignupCognito(string username, string password, string email)
        {
            SignUpResponse signUpResponse = await cognitoAuth.SignupUser(username, password, email);
            if (!signUpResponse.UserConfirmed) EmailVerificationNeeded = true;
        }

        public async Task VerifyUserCognito(string username, string verificationCode)
        {
            await cognitoAuth.VerifyAccessCode(username, verificationCode);
            EmailVerificationNeeded = false;
        }

        public void AuthenticateGoogle(IAuthenticationDelegate _delegate)
        {
            authDelegate = _delegate;
            googleAuth.AuthenticateGoogle(this);
            
        }

        public void OnGoogleAuthCanceled()
        {
            CurrentUser.Clear();
            authDelegate.OnAuthFailed("Authentication has been cancelled", null);
        }

        public async void OnGoogleAuthCompleted(Account account)
        {
            try
            {
                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    await GetGoogleUserinfo(account);
                    await GetAWSCredentialsForGoogleToken(account);
                }

                authDelegate.OnAuthCompleted();
            }
            catch(Exception ex)
            {
                CurrentUser.Clear();
                authDelegate.OnAuthFailed(ex.Message, ex);
            }
        }

        public void OnGoogleAuthFailed(string message, Exception exception)
        {
            CurrentUser.Clear();
            authDelegate.OnAuthFailed(message, exception);
        }

        private async Task GetGoogleUserinfo(Account account)
        {
            //get GOOGLE userinfo
            var req = new OAuth2Request("GET", new Uri("https://www.googleapis.com/oauth2/v2/userinfo"), null, account);
            var resp = await req.GetResponseAsync();
            var obj = JObject.Parse(resp.GetResponseText());
            CurrentUser.UserId = obj["id"].ToString().Replace("\"", "");
            CurrentUser.UserName = obj["given_name"].ToString().Replace("\"", "");
            CurrentUser.Email = obj["email"].ToString().Replace("\"", "");
            CurrentUser.PhotoUrl = obj["picture"].ToString().Replace("\"", "");
            CurrentUser.Type = MUser.MUserType.Google;
        }

        private async Task GetAWSCredentialsForGoogleToken(Account account)
        {
            //get AWS credentials
            GetCredentialsForIdentityResponse getCredentialResp = await cognitoAuth.GetCognitoIdentityWithGoogleToken(account.Properties["id_token"]);
            CurrentUser.AWSCredentials = getCredentialResp.Credentials;
            CurrentUser.CognitoIdentityId = getCredentialResp.IdentityId;
        }
    }
}
