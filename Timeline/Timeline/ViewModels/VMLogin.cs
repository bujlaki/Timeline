using Acr.UserDialogs;
using System;
using Timeline.Services;
using Xamarin.Forms;

namespace Timeline.ViewModels
{
    public class VMLogin : Base.VMBase, IAuthenticationDelegate
    {
        string username;
        string password;

        public Command CmdTest { get; set; }
        public Command CmdBazLogin { get; set; }
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

        public VMLogin() : base()
        {
            CmdTest = new Command(CmdTestExecute);
            CmdBazLogin = new Command(CmdBazLoginExecute);
            CmdGoogleLogin = new Command(CmdGoogleLoginExecute);
            CmdUserPassLogin = new Command(CmdUserPassLoginExecute);
            CmdForgotPassword = new Command(CmdForgotPasswordExecute);
            CmdSignup = new Command(CmdSignupExecute);
        }

        void CmdTestExecute(object obj)
        {
            App.services.Navigation.GoToTestPage();
        }

        async void CmdBazLoginExecute(object obj)
        {
            try
            {
                if (!Lock()) return;
                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    await App.services.Authentication.LoginCognito("baz", "password");
                }

                App.services.Database.Connect(App.services.Authentication.Login.AWSCredentials);
                App.services.Navigation.GoToUserPagesPage(App.services.Authentication.CurrentUser, true);
                Unlock();
            }
            catch (Exception ex)
            {
                Unlock();
                UserDialogs.Instance.Alert(ex.Message, "Login error");
            }
        }

        void CmdGoogleLoginExecute(object obj)
        {
            try
            {
                if (!Lock()) return;
                App.services.Authentication.AuthenticateGoogle(this);      
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
                    await App.services.Authentication.LoginCognito(username, password);
                }

                App.services.Database.Connect(App.services.Authentication.Login.AWSCredentials);
                App.services.Navigation.GoToUserPagesPage(App.services.Authentication.CurrentUser, true);
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
            App.services.Navigation.GoToSignupPage();
            Unlock();
        }

        public void OnAuthCompleted()
        {
            App.services.Database.Connect(App.services.Authentication.Login.AWSCredentials);
            App.services.Navigation.GoToUserPagesPage(App.services.Authentication.CurrentUser, true);
        }

        public void OnAuthFailed(string message, Exception exception)
        {

        }
    }
}
