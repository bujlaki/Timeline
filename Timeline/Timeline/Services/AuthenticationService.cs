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

        public LoginData Login { get; private set; }

        public AuthenticationService()
        {
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetCachedCredentials ERROR: " + ex.Message);

                throw ex;
            }
        }

        public async Task LoginCognito(string username, string password, IAuthenticationDelegate authDelegate)
        {
            try
            {
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);
                await cognitoAuth.ValidateUser(username, password, loginDataRef);
                Login.Type = LoginType.Cognito;

                authDelegate.OnAuthCompleted();
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoginCognito Exception: " + ex.Message);
                authDelegate.OnAuthFailed("Cognito authentication failed: " + ex.Message, ex);
            }
        }

        public async Task SignupCognito(string username, string password, string email, ISignupDelegate signupDelegate)
        {
            try
            {
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);
                await cognitoAuth.SignupUser(username, password, email, loginDataRef);

                signupDelegate.OnSignupCompleted();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SignupCognito Exception: " + ex.Message);
                signupDelegate.OnSignupFailed("Cognito signup failed: " + ex.Message, ex);
            }
        }

        public async Task VerifyUserCognito(string username, string verificationCode, IAccountVerificationDelegate verificationDelegate)
        {
            try
            {
                Ref<LoginData> loginDataRef = new Ref<LoginData>(Login);
                await cognitoAuth.VerifyAccessCode(username, verificationCode, loginDataRef);

                verificationDelegate.OnVerificationCompleted();
            }
            catch (Exception ex)
            {
                Console.WriteLine("VerifyUserCognito Exception: " + ex.Message);
                verificationDelegate.OnVerificationFailed("Cognito verification failed: " + ex.Message, ex);
            }
        }

        public void AuthenticateGoogle(IAuthenticationDelegate _delegate)
        {
            authDelegate = _delegate;
            platformGoogleAuth.AuthenticateGoogle(googleAuth);
        }

        public void SignOut()
        {
            Login.Clear();
            ClearCachedCredentials();
            googleAuth.ReInit();
        }

        public void OnGoogleAuthCanceled()
        {
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

                authDelegate.OnAuthCompleted();
            }
            catch(Exception ex)
            {
                Console.WriteLine("OnGoogleAuthCompleted ERROR: " + ex.Message);
                authDelegate.OnAuthFailed(ex.Message, ex);
            }
        }

        public void OnGoogleAuthFailed(string message, Exception exception)
        {
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

    }
}
