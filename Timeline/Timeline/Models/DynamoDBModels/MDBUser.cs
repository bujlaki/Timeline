using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2.DocumentModel;

namespace Timeline.Models.DynamoDBModels
{
    public class MDBUser
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<MDBTimelineInfo> Timelines { get; set; }

        public MDBUser()
        {
            UserId = "";
            UserName = "";
            Email = "";
            Timelines = new List<MDBTimelineInfo>();
        }

        public MDBUser(Document userdoc)
        {
            UserId = userdoc["userid"];
            UserName = userdoc["username"];
            Email = userdoc["email"];
            Timelines = new List<MDBTimelineInfo>();
            var listdoc = userdoc["timelines"].AsDocument();
            
            foreach (Document item in listdoc.Values) Timelines.Add(new MDBTimelineInfo(item));
        }

        public MDBUser(string id, string name, string email)
        {
            UserId = id;
            UserName = name;
            Email = email;
            Timelines = new List<MDBTimelineInfo>();
        }

        public MDBUser(MUser user)
        {
            UserId = user.UserId;
            UserName = user.UserName;
            Email = user.Email;
            Timelines = new List<MDBTimelineInfo>();
        }

        public Document AsDynamoDocument()
        {
            var doc = new Document();
            doc["userid"] = UserId;
            doc["username"] = UserName;
            doc["email"] = Email;
            Document timelines = new Document();
            foreach (MDBTimelineInfo tli in Timelines) timelines.Add(tli.TimelineId, tli.AsDynamoDocument());
            doc["timelines"] = timelines;            
            return doc;
        }
    }
}
