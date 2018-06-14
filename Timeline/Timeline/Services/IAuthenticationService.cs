using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Timeline.Objects.Auth.Google;

namespace Timeline.Services
{
    public interface IAuthenticationService
    {
        void AuthenticateGoogle(IAuthenticationDelegate _delegate);
        Task LoginCognito(IAuthenticationDelegate _delegate, string username, string password);
        Task SignupCognito(IAuthenticationDelegate _delegate, string username, string password, string email);
        Task VerifyUserCognito(IAuthenticationDelegate _delegate, string username, string code);
    }

    public interface IAuthenticationDelegate
    {
        void OnAuthCompleted(GoogleOAuthToken token);
        void OnAuthFailed(string message, Exception exception);
    }
}
