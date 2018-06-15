﻿using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

using Timeline.Objects.Auth.Google;
using Timeline.Services;
using Acr.UserDialogs;

namespace Timeline.ViewModels
{
    public class VMLogin : Base.VMBase, IAuthenticationDelegate
    {
        bool busy;
        string username;
        string password;

        public Command CmdGoogleLogin { get; set; }

        public Command CmdUserPassLogin { get; set; }

        public Command CmdForgotPassword { get; set; }

        public Command CmdSignup { get; set; }

        public bool Busy
        {
            get { return busy; }
            set { busy = value; RaisePropertyChanged("Busy"); }
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
            CmdForgotPassword = new Command(CmdForgotPasswordExecute);
            CmdSignup = new Command(CmdSignupExecute);

            //LoginResult = "TRY GMAIL";

            //CHECK CACHED COGNITO IDENTITY
            if (!DesignMode.IsDesignModeEnabled)
            {
                if (_services.Authentication.GetCachedCredentials())
                {
                    UserDialogs.Instance.Alert("Login successful. Cached credentials");
                }
            }
        }

        void CmdGoogleLoginExecute(object obj)
        {
            try
            {
                Busy = true;
                _services.Authentication.AuthenticateGoogle(this);
            }
            finally
            {
                Busy = false;
            }
        }

        async void CmdUserPassLoginExecute(object obj)
        {
            try
            {
                Busy = true;
                using (UserDialogs.Instance.Loading("Logging in..."))
                {
                    await _services.Authentication.LoginCognito(username, password);
                }

                UserDialogs.Instance.Alert("Login successful. Username: " + _services.Authentication.CurrentUser.UserName);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Alert(ex.Message, "Timeline Signup error");
            }
            finally
            {
                Busy = false;
            }
            
        }

        async void CmdForgotPasswordExecute(object obj)
        {
            
        }

        void CmdSignupExecute(object obj)
        {
            _services.Navigation.GoToSignupPage();
        }

        public void OnAuthCompleted()
        {
            UserDialogs.Instance.Alert("Login successful. CognitoIdentityId: " + _services.Authentication.CurrentUser.CognitoIdentityId);
        }

        public void OnAuthFailed(string message, Exception exception)
        {

        }
    }
}
