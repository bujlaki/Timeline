using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Objects.Auth.Google;
using Timeline.Services;

namespace Timeline.ViewModels
{
    public class VMLogin : Base.VMBase, IAuthenticationDelegate
    {
        string loginResult;
        string username;
        string password;

        public Command CmdGoogleLogin { get; set; }

        public Command CmdUserPassLogin { get; set; }

        public string LoginResult {
            get { return loginResult; }
            set { loginResult = value; RaisePropertyChanged("LoginResult"); }
        }

        public string Username
        {
            get { return username; }
            set { username = value; RaisePropertyChanged("Username"); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; RaisePropertyChanged("Password"); }
        }

        public VMLogin(Services.Base.ServiceContainer services) : base(services)
        {
            CmdGoogleLogin = new Command(CmdGoogleLoginExecute, CmdGoogleLoginCanExecute);
            CmdUserPassLogin = new Command(CmdUserPassLoginExecute, CmdUserPassLoginCanExecute);

            LoginResult = "TRY GMAIL";

            //CHECK CACHED COGNITO IDENTITY
            //Console.WriteLine("Checking cached Cognito credentials");
            //_services.Cognito.GetCachedCognitoIdentity();
            //if(_services.Cognito.IsLoggedIn) {
            //    LoginResult = "ALREADY LOGGED IN";
            //    Console.WriteLine("LOGGED IN AS: " + _services.Cognito.CognitoId);
            //}
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
            await _services.Authentication.LoginCognito(this, username, password);
        }

        bool CmdUserPassLoginCanExecute(object arg)
        {
            return true;
        }

        public void OnAuthCompleted(GoogleOAuthToken token)
        {
            LoginResult = "success";
        }

        public void OnAuthFailed(string message, Exception exception)
        {
            LoginResult = message;
        }
    }
}
