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

        public Command CmdSignup { get; set; }

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
            CmdGoogleLogin = new Command(CmdGoogleLoginExecute);
            CmdUserPassLogin = new Command(CmdUserPassLoginExecute);
            CmdSignup = new Command(CmdSignupExecute);

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

        async void CmdUserPassLoginExecute(object obj)
        {
            await _services.Authentication.LoginCognito(this, username, password);
        }

        void CmdSignupExecute(object obj)
        {
            _services.Navigation.GoToSignupPage();
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
