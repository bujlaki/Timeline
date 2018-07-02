using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2.DocumentModel;

namespace Timeline.Models.DynamoDBModels
{
    
    public class MDBTimelineInfo
    {
        public string TimelineId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public MDBTimelineInfo(string name, string description)
        {
            TimelineId = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
        }

        public MDBTimelineInfo(Document doc)
        {
            TimelineId = doc["timelineid"];
            Name = doc["name"];
            Description = doc["description"];
        }

        public Document AsDynamoDocument()
        {
            var doc = new Document();
            doc.Add("timelineid", TimelineId);
            doc.Add("name", Name);
            doc.Add("description", Description);
            //doc["timelineid"] = TimelineId;
            //doc["name"] = Name;
            //doc["description"] = Description;
            return doc;
        }
    }
}
