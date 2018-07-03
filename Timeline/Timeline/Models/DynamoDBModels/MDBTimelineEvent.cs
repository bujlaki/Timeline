using System;

using Amazon.DynamoDBv2.DocumentModel;

namespace Timeline.Models.DynamoDBModels
{
    public class MDBTimelineEvent
    {
        public string TimelineId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string URL { get; set; }
        public string Data { get; set; }
        public Int64 StartDate { get; set; }
        public Int64 EndDate { get; set; }
        public byte Precision { get; set; }

        public MDBTimelineEvent(string timelineId, string title, Int64 startDate, byte precision)
            : this(timelineId, title, "", "", "", "", startDate, 0, precision) { }

        public MDBTimelineEvent(string timelineId, string title, string description, Int64 startDate, byte precision)
            : this (timelineId,title,description,"","","",startDate,0,precision) { }

        public MDBTimelineEvent(string timelineId, string title, string description, string base64image, string url, string data, Int64 startDate, Int64 endDate, byte precision)
        {
            TimelineId = timelineId;
            Title = title;
            Description = description;
            Image = base64image;
            URL = url;
            Data = data;
            StartDate = startDate;
            EndDate = endDate;
            Precision = precision;
        }
                
        public MDBTimelineEvent(Document doc)
        {
            TimelineId = doc["timelineid"];
            Title = doc["title"];
            Description = doc["description"];
            Image = doc["image"];
            URL = doc["url"];
            Data = doc["data"];
            StartDate = Int64.Parse(doc["startdate"]);
            EndDate = Int64.Parse(doc["enddate"]);
            Precision = byte.Parse(doc["precision"]);
        }

        public Document AsDynamoDocument()
        {
            var doc = new Document();
            doc.Add("timelineid", TimelineId);
            doc.Add("title", Title);
            doc.Add("description", Description);
            doc.Add("image", Image);
            doc.Add("url", URL);
            doc.Add("data", Data);
            doc.Add("startdate", StartDate.ToString());
            doc.Add("enddate", EndDate.ToString());
            doc.Add("precision", Precision.ToString());
            return doc;
        }
    }
}
