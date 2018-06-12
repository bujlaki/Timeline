using Amazon.Extensions.CognitoAuthentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Timeline.Services
{
    public interface ICognitoService
    {
        string CognitoId { get; }
        bool IsLoggedIn { get; }

        void GetCachedCognitoIdentity();
        void GetCognitoIdentityWithGoogleToken(string token);
        void GetCognitoIdentityWithUserPass(string username, string password);
        Task<CognitoUser> ValidateUser(string username, string password);
    }
}
