using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Auth.Google
{
    public interface IPlatformSpecificGoogleAuth
    {
        string PlatformClientID { get; }
        void AuthenticateGoogle(GoogleAuthenticator googleAuthenticator);
        void ClearStaticAuth();
    }
}
