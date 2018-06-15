using System;
using System.Collections.Generic;
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

        public bool GetCachedCredentials()
        {
            CurrentUser.AWSCredentials = cognitoAuth.GetCachedCognitoIdentity();
            if (CurrentUser.AWSCredentials == null)
            {
                CurrentUser.Clear();
                return false;
            }
            CurrentUser.Type = MUser.MUserType.Google;
            
            return true;
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

        public async void OnGoogleAuthCompleted(GoogleOAuthToken token)
        {
            try
            {
                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    GetCredentialsForIdentityResponse getCredentialResp = await cognitoAuth.GetCognitoIdentityWithGoogleToken(token.IdToken);
                
                    CurrentUser.AWSCredentials = getCredentialResp.Credentials;
                    CurrentUser.CognitoIdentityId = getCredentialResp.IdentityId;
                    CurrentUser.Type = MUser.MUserType.Google;
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


    }
}
