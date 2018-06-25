using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Timeline.Models;
using Timeline.Objects.Auth.Google;

namespace Timeline.Services
{
    public interface IAuthenticationService
    {
        MUser CurrentUser { get; }
        bool EmailVerificationNeeded { get; }
        Task<bool> GetCachedCredentials();
        void ClearCachedCredentials();
        void AuthenticateGoogle(IAuthenticationDelegate _delegate);
        Task LoginCognito(string username, string password);
        Task SignupCognito(string username, string password, string email);
        Task VerifyUserCognito(string username, string verificationCode);
    }

    public interface IAuthenticationDelegate
    {
        void OnAuthCompleted();
        void OnAuthFailed(string message, Exception exception);
    }
}
