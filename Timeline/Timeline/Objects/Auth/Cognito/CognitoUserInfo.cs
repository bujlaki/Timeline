using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Auth.Cognito
{
    public class CognitoUserInfo
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }

        public LoginType Type { get; set; }
        public string TypeStr { get { return "COGNITO"; } }

        public bool SignupVerified { get; set; }               //for Cognito UserPool email-verification

        public Xamarin.Auth.Account Account { get; set; }      //to hold tokens

        public string IdentityId { get; set; }                 //Cognito IdentityPool - IdentityId
        public AWSCredentials AWSCredentials { get; set; }     //Credentials to access AWS services

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
            AWSCredentials = null;
            SignupVerified = false;
        }
    }
}
