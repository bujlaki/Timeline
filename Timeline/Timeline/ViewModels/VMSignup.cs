using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Services;
using Timeline.Objects.Auth.Google;
using Acr.UserDialogs;

namespace Timeline.ViewModels
{
    public class VMSignup : Base.VMBase, IAuthenticationDelegate
    {

        string loginResult;
        string username;
        string password;
        string email;

        public Command CmdSignup { get; set; }

        public string LoginResult
        {
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

        public string Email
        {
            get { return email; }
            set { email = value; RaisePropertyChanged("Email"); }
        }

        public VMSignup(Services.Base.ServiceContainer services) : base(services)
        {            
            CmdSignup = new Command(CmdSignupExecute);
        }

        async void CmdSignupExecute(object obj)
        {
            await _services.Authentication.SignupCognito(this, "balazs2", "Password123", "balazs.ujlaki@gmail.com");
            PromptConfig pc = new PromptConfig
            {
                OnTextChanged = args => {
                    args.IsValid = true; // setting this to false will disable the OK/Positive button
                    //args.Text = ""; // you can read the current value as well as replace the textbox value here
                }
            };
            PromptResult pr = await UserDialogs.Instance.PromptAsync(pc);

            await _services.Authentication.VerifyUserCognito(this, "balazs2", pr.Text);
        }

        public void OnAuthCompleted(GoogleOAuthToken token)
        {
            throw new NotImplementedException();
        }

        public void OnAuthFailed(string message, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
