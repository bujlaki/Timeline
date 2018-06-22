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
            try
            {
                using (UserDialogs.Instance.Loading("Please wait..."))
                {
                    await _services.Authentication.SignupCognito(username, password, email);
                }

                if (_services.Authentication.EmailVerificationNeeded)
                {
                    PromptConfig pc = new PromptConfig
                    {    
                        Title = "Check your email for the verification code!",
                    };
                    
                    PromptResult pr = await UserDialogs.Instance.PromptAsync(pc);

                    using (UserDialogs.Instance.Loading("Confirming verification code..."))
                    {
                        await _services.Authentication.VerifyUserCognito(username, pr.Text);
                    }
                }

                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    await _services.Authentication.LoginCognito(username, password);
                }

                //SUCCESS
                _services.Navigation.GoToUserPagesPage(true);
            }
            catch (Exception ex)
            {
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
