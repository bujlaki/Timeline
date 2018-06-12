using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Objects.Auth.Google;

namespace Timeline.ViewModels
{
    public class VMLogin : Base.VMBase, IGoogleAuthenticationDelegate
    {
        string loginResult;

        public Command CmdGoogleLogin { get; set; }

        public Command CmdUserPassLogin { get; set; }

        public string LoginResult {
            get { return loginResult; }
            set { loginResult = value; RaisePropertyChanged("LoginResult"); }
        }

        public VMLogin(Services.Base.ServiceContainer services) : base(services)
        {
            CmdGoogleLogin = new Command(CmdGoogleLoginExecute, CmdGoogleLoginCanExecute);
            CmdUserPassLogin = new Command(CmdUserPassLoginExecute, CmdUserPassLoginCanExecute);

            LoginResult = "TRY GMAIL";

            Console.WriteLine("Checking cached Cognito credentials");
            _services.Cognito.GetCachedCognitoIdentity();
            if(_services.Cognito.IsLoggedIn) {
                LoginResult = "ALREADY LOGGED IN";
                Console.WriteLine("LOGGED IN AS: " + _services.Cognito.CognitoId);
            }
        }

        void CmdGoogleLoginExecute(object obj)
        {
            _services.Authentication.AuthenticateGoogle(this);
        }

        bool CmdGoogleLoginCanExecute(object arg)
        {
            return true;
        }

        async void CmdUserPassLoginExecute(object obj)
        {
            try
            {
                await _services.Cognito.ValidateUser("username", "userPassword123");
            }
            catch (Exception ex)
            {
                LoginResult = ex.Message;
            }
        }

        bool CmdUserPassLoginCanExecute(object arg)
        {
            return true;
        }

        public void OnAuthenticationCompleted(GoogleOAuthToken token)
        {
            LoginResult = "SUCCESS";
            _services.Cognito.GetCognitoIdentityWithGoogleToken(token.IdToken);

            if (_services.Cognito.IsLoggedIn)
            {
                LoginResult = "COGNITO SUCCESS";
                Console.WriteLine("LOGGED IN AS: " + _services.Cognito.CognitoId);
            }
        }

        public void OnAuthenticationFailed(string message, Exception exception)
        {
            LoginResult = "FAILED";
        }

        public void OnAuthenticationCanceled()
        {
            LoginResult = "CANCELLED";
        }
    }
}
