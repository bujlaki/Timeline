using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Services
{
    public interface ICognitoService
    {
        string CognitoId { get; }
        bool IsLoggedIn { get; }

        void GetCachedCognitoIdentity();
        void GetCognitoIdentityWithGoogleToken(string token);
    }
}
