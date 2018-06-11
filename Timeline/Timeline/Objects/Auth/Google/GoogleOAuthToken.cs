using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Auth.Google
{
    public class GoogleOAuthToken
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
    }
}
