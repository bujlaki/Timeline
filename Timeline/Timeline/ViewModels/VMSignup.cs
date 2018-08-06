using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Timeline.Services;
using Acr.UserDialogs;

namespace Timeline.ViewModels
{
    public class VMSignup : Base.VMBase, IAuthenticationDelegate, ISignupDelegate, IAccountVerificationDelegate
    {
        string username;
        string password;
        string email;

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

        public string Email
        {
            get { return email; }
            set { email = value; RaisePropertyChanged("Email"); }
        }

        public VMSignup() : base()
        {            
            CmdSignup = new Command(CmdSignupExecute);
        }

        void CmdSignupExecute(object obj)
        {
            Busy = true;
            BusyMessage = "Signing up...";
            Task.Run(async () =>
            {
                await App.services.Authentication.SignupCognito(username, password, email, this);
            });
        }

        public void OnAuthCompleted()
        {
            //SUCCESS
            App.services.Database.Connect(App.services.Authentication.Login.AWSCredentials);
            Task.Run(async () =>
            {
                await App.services.Database.CreateUser(App.services.Authentication.Login);
            }).Wait();
            App.services.Navigation.GoToUserPagesPage(App.services.Authentication.Login.UserId, true);
            Busy = false;
            Unlock();
        }

        public void OnAuthFailed(string message, Exception exception)
        {
            Busy = false;
        }

        public void OnSignupCompleted()
        {
            Busy = false;
            PromptConfig pc = new PromptConfig
            {
                Title = "Check your email for the verification code!",
            };

            PromptResult pr;
            Task.Run(async () =>
            {
                pr = await UserDialogs.Instance.PromptAsync(pc);

                BusyMessage = "Confirming verification code...";
                Busy = true;
                await App.services.Authentication.VerifyUserCognito(username, pr.Text, this);
            });
        }

        public void OnSignupFailed(string message, Exception exception)
        {
            Busy = false;
            UserDialogs.Instance.Alert(message, "Signup failed");
        }

        public void OnVerificationCompleted()
        {
            BusyMessage = "Logging in...";
            Task.Run(async () =>
            {
                await App.services.Authentication.LoginCognito(username, password, this);
            });
        }

        public void OnVerificationFailed(string message, Exception exception)
        {
            Busy = false;
            UserDialogs.Instance.Alert(message, "Account verification failed");
        }
    }
}
