using System;
using System.Collections.Generic;
using System.Text;

using Amazon.Runtime;

namespace Timeline.Objects.Auth
{
    public enum LoginType
    {
        None,
        Cognito,
        Google
    }

    public class LoginData
    {
        public class LoginAccount
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string id_token { get; set; }
            public string refresh_token { get; set; }
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }

        public LoginType Type { get; set; }
        public string TypeStr {
            get
            {
                if (Type == LoginType.Cognito) return "COGNITO";
                if (Type == LoginType.Google) return "GOOGLE";
                return "";
            }
        }

        public bool SignupVerified { get; set; }                //for Cognito UserPool email-verification

        public LoginAccount Account { get; set; }               //to hold tokens

        public string IdentityId { get; set; }                  //Cognito IdentityPool - IdentityId
        public AWSCredentials AWSCredentials { get; set; }      //Credentials to access AWS services

        public LoginData()
        {
            Clear();
        }

        public override string ToString()
        {
            return TypeStr;
        }

        public void Clear()
        {
            UserId = "";
            UserName = "";
            Email = "";
            Picture = "";

            Type = LoginType.None;

            SignupVerified = false;

            Account = null;

            IdentityId = "";
            AWSCredentials = null;   
        }
    }
}
