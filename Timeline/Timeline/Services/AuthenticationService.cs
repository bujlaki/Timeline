using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Auth;

using Timeline.Models;
using Timeline.Objects;
using Timeline.Objects.Auth;
using Timeline.Objects.Auth.Cognito;
using Timeline.Objects.Auth.Google;

namespace Timeline.Services
{
    class AuthenticationService : IAuthenticationService, IGoogleAuthenticationDelegate
    {
        private GoogleAuthenticator googleAuth;
        private CognitoAuthenticator cognitoAuth;

        private IPlatformSpecificGoogleAuth platformGoogleAuth;
        private IAuthenticationDelegate authDelegate;

        public MUser CurrentUser { get; private set; }
        public LoginData Login { get; private set; }

        public AuthenticationService()
        {
            CurrentUser = new MUser();
            Login = new LoginData();
            platformGoogleAuth = DependencyService.Get<IPlatformSpecificGoogleAuth>();
            googleAuth = new GoogleAuthenticator(platformGoogleAuth.PlatformClientID, "email profile", "hu.iqtech.timeline:/oauth2redirect", this);
            cognitoAuth = new CognitoAuthenticator();
        }

        public async Task GetCachedCredentials()
        {
            try
            {
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);

                IEnumerable<Account> accounts = AccountStore.Create().FindAccountsForService("Google");
                Account account = accounts.FirstOrDefault();
                if (account == null) return;

                await googleAuth.GetGoogleUserInfo(account, loginDataRef);
                await cognitoAuth.GetAWSCredentialsWithGoogleToken(account.Properties["id_token"], loginDataRef);
                Login.Type = LoginType.Google;
                PopulateUser();              
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
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);

                await cognitoAuth.ValidateUser(username, password, loginDataRef);
                Login.Type = LoginType.Cognito;
                PopulateUser();
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
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);
                CurrentUser.Clear();
                await cognitoAuth.SignupUser(username, password, email, loginDataRef);
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
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);
                await cognitoAuth.VerifyAccessCode(username, verificationCode, loginDataRef);
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
            Login.Clear();
            ClearCachedCredentials();
            googleAuth.ReInit();
        }

        public void OnGoogleAuthCanceled()
        {
            CurrentUser.Clear();
            Login.Clear();
            authDelegate.OnAuthFailed("Authentication has been cancelled", null);
        }

        public async void OnGoogleAuthCompleted(Account account)
        {
            try
            {
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);

                platformGoogleAuth.ClearStaticAuth();

                await googleAuth.GetGoogleUserInfo(account, loginDataRef);               
                await cognitoAuth.GetAWSCredentialsWithGoogleToken(account.Properties["id_token"], loginDataRef);
                Login.Type = LoginType.Google;
                PopulateUser();

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
            Login.Clear();
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

        private void PopulateUser()
        {
            CurrentUser.UserId = Login.UserId;
            CurrentUser.UserName = Login.UserName;
            CurrentUser.Email = Login.Email;
            CurrentUser.PhotoUrl = Login.Picture;
        }
    }
}
