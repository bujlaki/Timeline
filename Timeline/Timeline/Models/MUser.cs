using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Auth;

namespace Timeline.Models
{
    public class MUser
    {
        public enum MUserType
        {
            None,
            Cognito,
            Google
        }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public string CognitoIdentityId { get; set; }
        public MUserType Type { get; set; }
        public Account GoogleAccount { get; set; }
        public AWSCredentials AWSCredentials { get; set; }
        

        public MUser()
        {
            Clear();
        }

        public void Clear()
        {
            UserId = "";
            UserName = "";
            Email = "";
            PhotoUrl = "";
            CognitoIdentityId = "";
            Type = MUserType.None;
            GoogleAccount = null;
            AWSCredentials = null;
        }
    }
}
