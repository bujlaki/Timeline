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
        Task SignupCognito(string username, string password, string email);
        Task VerifyUserCognito(string username, string verificationCode);
        void SignOut();
    }

    public interface IAuthenticationDelegate
    {
        void OnAuthCompleted();
        void OnAuthFailed(string message, Exception exception);
    }
}
