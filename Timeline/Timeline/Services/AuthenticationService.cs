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
        private GoogleAuthenticator googleAuth;
        private CognitoAuthenticator cognitoAuth;

        private IPlatformSpecificGoogleAuth platformGoogleAuth;
        private IAuthenticationDelegate authDelegate;

        public MUser CurrentUser { get; private set; }
        public bool EmailVerificationNeeded { get; private set; }

        public AuthenticationService()
        {
            CurrentUser = new MUser();
            EmailVerificationNeeded = false;

            platformGoogleAuth = DependencyService.Get<IPlatformSpecificGoogleAuth>();

            googleAuth = new GoogleAuthenticator(platformGoogleAuth.PlatformClientID, "email profile", "hu.iqtech.timeline:/oauth2redirect", this);

            cognitoAuth = new CognitoAuthenticator();
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
                    var info = await googleAuth.GetGoogleUserInfo(account);
                    CurrentUser.UserId = info.UserId;
                    CurrentUser.UserName = info.UserName;
                    CurrentUser.Email = info.Email;
                    CurrentUser.PhotoUrl = info.Picture;
                    CurrentUser.Type = MUser.MUserType.Google;

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

        public void ClearCachedCredentials()
        {
            AccountStore accountStore = AccountStore.Create();
            IEnumerable<Account> accounts = accountStore.FindAccountsForService("Google");
            Account account = accounts.FirstOrDefault();
            if (account == null) return;

            accountStore.Delete(account, "Google");
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
            platformGoogleAuth.AuthenticateGoogle(googleAuth);
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
                platformGoogleAuth.ClearStaticAuth();

                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    var info = await googleAuth.GetGoogleUserInfo(account);
                    CurrentUser.UserId = info.UserId;
                    CurrentUser.UserName = info.UserName;
                    CurrentUser.Email = info.Email;
                    CurrentUser.PhotoUrl = info.Picture;
                    CurrentUser.Type = MUser.MUserType.Google;

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



        private async Task GetAWSCredentialsForGoogleToken(Account account)
        {
            //get AWS credentials
            GetCredentialsForIdentityResponse getCredentialResp = await cognitoAuth.GetCognitoIdentityWithGoogleToken(account.Properties["id_token"]);
            CurrentUser.AWSCredentials = getCredentialResp.Credentials;
            CurrentUser.CognitoIdentityId = getCredentialResp.IdentityId;
        }
    }
}
