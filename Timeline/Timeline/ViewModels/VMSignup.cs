using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Services;
using Timeline.Objects.Auth.Google;
using Acr.UserDialogs;
using System.Threading.Tasks;

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
            UserDialogs.Instance.ShowLoading("Please wait");

            try
            {    
                await _services.Authentication.SignupCognito(username, password, email);

                PromptConfig pc = new PromptConfig
                {
                    OnTextChanged = args => {
                        args.IsValid = true; // setting this to false will disable the OK/Positive button
                    }
                };
                Task<PromptResult> task = UserDialogs.Instance.PromptAsync(pc);
                task.Wait();            

                await _services.Authentication.VerifyUserCognito(username, task.Result.Text);

                UserDialogs.Instance.HideLoading();
            }
            catch(Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                UserDialogs.Instance.Alert(ex.Message, "Timeline Signup error");
            }
        }

        public void OnAuthCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnAuthFailed(string message, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
