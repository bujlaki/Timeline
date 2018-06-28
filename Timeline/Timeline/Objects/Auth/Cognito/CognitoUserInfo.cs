using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Auth.Cognito
{
    public class CognitoUserInfo
    {
        public string UserId { get; set; }
        public string IdentityId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public AWSCredentials Credentials { get; set; }
        public bool Verified { get; set; }

        public CognitoUserInfo()
        {
            Clear();
        }

        public void Clear()
        {
            UserId = "";
            IdentityId = "";
            UserName = "";
            Email = "";
            Picture = "";
            Credentials = null;
            Verified = false;
        }
    }
}
