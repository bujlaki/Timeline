using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Objects.Auth.Google;

namespace Timeline.ViewModels
{
    public class VMLogin : Base.VMBase, IGoogleAuthenticationDelegate
    {
        string loginResult;

        public Command CmdGoogleLogin { get; set; }

        public string LoginResult {
            get { return loginResult; }
            set { loginResult = value; RaisePropertyChanged("LoginResult"); }
        }

        public VMLogin(Services.Base.ServiceContainer services) : base(services)
        {
            CmdGoogleLogin = new Command(CmdGoogleLoginExecute, CmdGoogleLoginCanExecute);
            LoginResult = "TRY GMAIL";
        }

        void CmdGoogleLoginExecute(object obj)
        {
            _services.Authentication.AuthenticateGoogle(this);
        }

        bool CmdGoogleLoginCanExecute(object arg)
        {
            return true;
        }

        public void OnAuthenticationCompleted(GoogleOAuthToken token)
        {
            LoginResult = "SUCCESS";
        }

        public void OnAuthenticationFailed(string message, Exception exception)
        {
            LoginResult = "FAILED";
        }

        public void OnAuthenticationCanceled()
        {
            LoginResult = "CANCELLED";
        }
    }
}
