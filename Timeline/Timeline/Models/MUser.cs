using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Models
{
    class MUser
    {
        public enum MUserType
        {
            None,
            Cognito,
            Google
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public MUserType Type { get; set; }
        public string AWSToken { get; set; }
    }
}
