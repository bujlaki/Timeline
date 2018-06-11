using System;
using System.Collections.Generic;
using System.Text;
using Timeline.Objects.Auth.Google;

namespace Timeline.Services
{
    public interface IAuthenticationService
    {
        void AuthenticateGoogle(IGoogleAuthenticationDelegate _delegate);
    }
}
