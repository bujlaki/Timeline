using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Services;
using Acr.UserDialogs;

namespace Timeline.ViewModels
{
    public class VMLogin : Base.VMBase, IAuthenticationDelegate
    {
        string username;
        string password;

        public Command CmdDebugClearCachedID { get; set; }
        public Command CmdGoogleLogin { get; set; }
        public Command CmdUserPassLogin { get; set; }
        public Command CmdForgotPassword { get; set; }
        public Command CmdSignup { get; set; }

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
            CmdDebugClearCachedID = new Command(CmdDebugClearCachedIDExecute);
            CmdGoogleLogin = new Command(CmdGoogleLoginExecute);
            CmdUserPassLogin = new Command(CmdUserPassLoginExecute);
            CmdForgotPassword = new Command(CmdForgotPasswordExecute);
            CmdSignup = new Command(CmdSignupExecute);
        }

        void CmdDebugClearCachedIDExecute(object obj)
        {
            _services.Authentication.SignOut();
        }

        void CmdGoogleLoginExecute(object obj)
        {
            try
            {
                if (!Lock()) return;
                _services.Authentication.AuthenticateGoogle(this);      
                Unlock();
            }
            catch (Exception ex)
            {
                Unlock();
                UserDialogs.Instance.Alert(ex.Message, "Login error");
            }
        }

        async void CmdUserPassLoginExecute(object obj)
        {
            try
            {
                if (!Lock()) return;
                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    await _services.Authentication.LoginCognito(username, password);
                }

                _services.Navigation.GoToUserPagesPage(_services.Authentication.CurrentUser, true);
                Unlock();
            }
            catch (Exception ex)
            {
                Unlock();
                UserDialogs.Instance.Alert(ex.Message, "Login error");
            }
            
        }

        async void CmdForgotPasswordExecute(object obj)
        {
            if (!Lock()) return;

            Unlock();
        }

        void CmdSignupExecute(object obj)
        {
            if (!Lock()) return;
            _services.Navigation.GoToSignupPage();
            Unlock();
        }

        public void OnAuthCompleted()
        {
            _services.Navigation.GoToUserPagesPage(_services.Authentication.CurrentUser, true);
        }

        public void OnAuthFailed(string message, Exception exception)
        {

        }
    }
}
