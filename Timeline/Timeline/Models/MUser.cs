using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

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
        public string CognitoIdentityId { get; set; }
        public MUserType Type { get; set; }
        public AWSCredentials AWSCredentials { get; set; }
        

        public MUser()
        {
            Clear();
        }

        public void Clear()
        {
            UserId = "";
            UserName = "";
            CognitoIdentityId = "";
            Type = MUserType.None;
            AWSCredentials = null;
        }
    }
}
