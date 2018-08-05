using Acr.UserDialogs;
using System;
using System.Threading.Tasks;
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

        void CmdBazLoginExecute(object obj)
        {
            Busy = true;
            BusyMessage = "Logging in...";

            Task.Run(async () =>
            {
                await App.services.Authentication.LoginCognito("baz", "password", this);
            });
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

        void CmdUserPassLoginExecute(object obj)
        {
            Busy = true;
            BusyMessage = "Logging in...";

            Task.Run(async () =>
            {
                await App.services.Authentication.LoginCognito(username, password, this);
            });
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
            App.services.Navigation.GoToUserPagesPage(App.services.Authentication.Login.UserId, true);
            Busy = false;
            Unlock();
        }

        public void OnAuthFailed(string message, Exception exception)
        {
            Busy = false;
            Unlock();
            UserDialogs.Instance.Alert(exception.Message, "Login error");
        }
    }
}
