using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Auth.Google
{
    public interface IPlatformSpecificGoogleAuth
    {
        void AuthenticateGoogle(IGoogleAuthenticationDelegate _delegate);
    }
}
