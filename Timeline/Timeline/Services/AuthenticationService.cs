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

        public AuthenticationService()
        {
            CurrentUser = new MUser();

            platformGoogleAuth = DependencyService.Get<IPlatformSpecificGoogleAuth>();
            googleAuth = new GoogleAuthenticator(platformGoogleAuth.PlatformClientID, "email profile", "hu.iqtech.timeline:/oauth2redirect", this);
            cognitoAuth = new CognitoAuthenticator();
        }

        public async Task GetCachedCredentials()
        {
            try
            {
                IEnumerable<Account> accounts = AccountStore.Create().FindAccountsForService("Google");
                Account account = accounts.FirstOrDefault();
                if (account == null) return;

                await googleAuth.GetGoogleUserInfo(account);
                CurrentUser.UserId = googleAuth.UserInfo.UserId;
                CurrentUser.UserName = googleAuth.UserInfo.UserName;
                CurrentUser.Email = googleAuth.UserInfo.Email;
                CurrentUser.PhotoUrl = googleAuth.UserInfo.Picture;
                CurrentUser.Type = MUser.MUserType.Google;

                await cognitoAuth.GetAWSCredentialsWithGoogleToken(account.Properties["id_token"]);
                CurrentUser.AWSCredentials = cognitoAuth.UserInfo.Credentials;
                CurrentUser.CognitoIdentityId = cognitoAuth.UserInfo.IdentityId;

                CurrentUser.LoggedIn = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetCachedCredentials ERROR: " + ex.Message);
                CurrentUser.Clear();

                throw ex;
            }
        }

        public async Task LoginCognito(string username, string password)
        {
            try
            {
                await cognitoAuth.ValidateUser(username, password);
                CurrentUser.UserId = cognitoAuth.UserInfo.UserId;
                CurrentUser.UserName = cognitoAuth.UserInfo.UserName;
                CurrentUser.Email = cognitoAuth.UserInfo.Email;
                CurrentUser.PhotoUrl = cognitoAuth.UserInfo.Picture;
                CurrentUser.AWSCredentials = cognitoAuth.UserInfo.Credentials;
                CurrentUser.Type = MUser.MUserType.Cognito;
                CurrentUser.LoggedIn = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginCognito Exception: " + ex.Message);
                throw ex;
            }
        }

        public async Task SignupCognito(string username, string password, string email)
        {
            try
            {
                CurrentUser.Clear();
                await cognitoAuth.SignupUser(username, password, email);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignupCognito Exception: " + ex.Message);
                throw ex;
            }
        }

        public async Task VerifyUserCognito(string username, string verificationCode)
        {
            try
            {
                await cognitoAuth.VerifyAccessCode(username, verificationCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("VerifyUserCognito Exception: " + ex.Message);
                throw ex;
            }
        }

        public void AuthenticateGoogle(IAuthenticationDelegate _delegate)
        {
            authDelegate = _delegate;
            platformGoogleAuth.AuthenticateGoogle(googleAuth);
        }

        public void SignOut()
        {
            CurrentUser.Clear();
            ClearCachedCredentials();
            googleAuth.ReInit();
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

                await googleAuth.GetGoogleUserInfo(account);
                CurrentUser.UserId = googleAuth.UserInfo.UserId;
                CurrentUser.UserName = googleAuth.UserInfo.UserName;
                CurrentUser.Email = googleAuth.UserInfo.Email;
                CurrentUser.PhotoUrl = googleAuth.UserInfo.Picture;
                CurrentUser.Type = MUser.MUserType.Google;

                await cognitoAuth.GetAWSCredentialsWithGoogleToken(account.Properties["id_token"]);
                CurrentUser.AWSCredentials = cognitoAuth.UserInfo.Credentials;
                CurrentUser.CognitoIdentityId = cognitoAuth.UserInfo.IdentityId;

                CurrentUser.LoggedIn = true;
                authDelegate.OnAuthCompleted();
            }
            catch(Exception ex)
            {
                Console.WriteLine("OnGoogleAuthCompleted ERROR: " + ex.Message);
                CurrentUser.Clear();
                authDelegate.OnAuthFailed(ex.Message, ex);
            }
        }

        public void OnGoogleAuthFailed(string message, Exception exception)
        {
            CurrentUser.Clear();
            authDelegate.OnAuthFailed(message, exception);
        }

        public void ClearCachedCredentials()
        {
            AccountStore accountStore = AccountStore.Create();
            IEnumerable<Account> accounts = accountStore.FindAccountsForService("Google");
            Account account = accounts.FirstOrDefault();
            if (account == null) return;

            accountStore.Delete(account, "Google");
        }
    }
}
