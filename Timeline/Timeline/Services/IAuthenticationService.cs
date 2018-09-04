using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Timeline.Models;
using Timeline.Objects.Auth;
using Timeline.Objects.Auth.Google;

namespace Timeline.Services
{
    public interface IAuthenticationService
    {
        LoginData Login { get; }
        Task GetCachedCredentials();
        void AuthenticateGoogle(IAuthenticationDelegate _delegate);
        Task LoginCognito(string username, string password, IAuthenticationDelegate authDelegate);
        Task SignupCognito(string username, string password, string email, ISignupDelegate signupDelegate);
        Task VerifyUserCognito(string username, string verificationCode, IAccountVerificationDelegate verificationDelegate);
        void SignOut();
        Task DeleteCognitoUser(string username);
    }

    public interface IAuthenticationDelegate
    {
        void OnAuthCompleted();
        void OnAuthFailed(string message, Exception exception);
    }

    public interface ISignupDelegate
    {
        void OnSignupCompleted();
        void OnSignupFailed(string message, Exception exception);
    }

    public interface IAccountVerificationDelegate
    {
        void OnVerificationCompleted();
        void OnVerificationFailed(string message, Exception exception);
    }
}
