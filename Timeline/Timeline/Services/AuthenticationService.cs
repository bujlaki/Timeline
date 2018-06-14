using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Amazon.Extensions.CognitoAuthentication;

using Timeline.Models;
using Timeline.Objects.Auth.Cognito;
using Timeline.Objects.Auth.Google;

namespace Timeline.Services
{
    class AuthenticationService : IAuthenticationService, IGoogleAuthenticationDelegate
    {
        public MUser currentUser;

        private IPlatformSpecificGoogleAuth googleAuth;
        private CognitoAuthenticator cognitoAuth;
        private IAuthenticationDelegate authDelegate;

        public AuthenticationService()
        {
            currentUser = new MUser();
            currentUser.Type = MUser.MUserType.None;

            cognitoAuth = new CognitoAuthenticator();
            googleAuth = DependencyService.Get<IPlatformSpecificGoogleAuth>();
        }

        public async Task LoginCognito(IAuthenticationDelegate _delegate, string username, string password)
        {
            try
            {
                CognitoUser user = await cognitoAuth.ValidateUser(username, password);
            }
            catch(Exception ex)
            {
                _delegate.OnAuthFailed(ex.Message, ex);
            }
        }

        public async Task SignupCognito(IAuthenticationDelegate _delegate, string username, string password, string email)
        {
            try
            {
                CognitoUser user = await cognitoAuth.SignupUser(username, password, email);
            }
            catch(Exception ex)
            {
                _delegate.OnAuthFailed(ex.Message, ex);
            }
        }

        public async Task VerifyUserCognito(IAuthenticationDelegate _delegate, string username, string code)
        {
            try
            {
                CognitoUser user = await cognitoAuth.VerifyAccessCode(username, code);
            }
            catch (Exception ex)
            {
                _delegate.OnAuthFailed(ex.Message, ex);
            }
        }

        public void AuthenticateGoogle(IAuthenticationDelegate _delegate)
        {
            authDelegate = _delegate;
            googleAuth.AuthenticateGoogle(this);
        }

        public void OnGoogleAuthCanceled()
        {
            authDelegate.OnAuthFailed("Authentication has been cancelled", null);
        }

        public void OnGoogleAuthCompleted(GoogleOAuthToken token)
        {
            //CONTINUE COGNITO AUTH
            //_services.Cognito.GetCognitoIdentityWithGoogleToken(token.IdToken);

            //if (_services.Cognito.IsLoggedIn)
            //{
            //    LoginResult = "COGNITO SUCCESS";
            //    Console.WriteLine("LOGGED IN AS: " + _services.Cognito.CognitoId);
            //}
        }

        public void OnGoogleAuthFailed(string message, Exception exception)
        {
            authDelegate.OnAuthFailed(message, exception);
        }


    }
}
