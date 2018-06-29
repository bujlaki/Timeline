using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2.DataModel;

namespace Timeline.Models.DynamoDBModels
{
    [DynamoDBTable("TimelineUsers")]
    public class MDBUser
    {
        [DynamoDBHashKey]
        public string userid { get; set; }

        public string username { get; set; }

        public string email { get; set; }

        public MDBUser()
        {
            userid = "";
            username = "";
            email = "";
        }

        public MDBUser(string id, string name, string email)
        {
            userid = id;
            username = name;
            this.email = email;
        }

        public MDBUser(MUser user)
        {
            userid = user.UserId;
            username = user.UserName;
            this.email = user.Email;
        }
    }
}
