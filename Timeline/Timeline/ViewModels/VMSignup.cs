using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Timeline.Services;
using Acr.UserDialogs;
using System.IO;
using System.Reflection;

namespace Timeline.ViewModels
{
    public class VMSignup : Base.VMBase, IAuthenticationDelegate, ISignupDelegate, IAccountVerificationDelegate
    {
        string username;
        string password;
        string email;
        bool policyAccepted;

        public bool PolicyAccepted {
            get { return policyAccepted; }
            set { policyAccepted = value; RaisePropertyChanged("PolicyAccepted"); }
        }

        public Command CmdPrivacyPolicy { get; set; }
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
            CmdPrivacyPolicy = new Command(CmdPrivacyPolicyExecute);
        }

        void CmdSignupExecute(object obj)
        {
            if(!PolicyAccepted)
            {
                AlertConfig ac = new AlertConfig();
                ac.Title = "Your consent is required";
                ac.Message = "You have to accept the Privacy Policy to create a new Timeline account.";
                UserDialogs.Instance.Alert(ac);
                return;
            }

            Busy = true;
            BusyMessage = "Signing up...";
            Task.Run(async () =>
            {
                await App.services.Authentication.SignupCognito(username, password, email, this);
            });
        }

        void CmdPrivacyPolicyExecute(object obj)
        {
            byte[] buffer;
            string strpp;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Timeline.Embedded.Text.PrivacyPolicy.txt"))
            {
                long length = s.Length;
                buffer = new byte[length];
                s.Read(buffer, 0, (int)length);
                strpp = System.Text.Encoding.Default.GetString(buffer);
            }

            AlertConfig ac = new AlertConfig();
            ac.Title = "Privacy Policy";
            ac.Message = strpp;

            UserDialogs.Instance.Alert(ac);
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
