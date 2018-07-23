using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2.DocumentModel;
using Timeline.Models;

namespace Timeline.Objects.Database
{
    class DynamoDBAdapter
    {
        public static Document User2DynamoDoc(MUser user)
        {
            var doc = new Document();
            doc.Add("userid", user.UserId);
            doc.Add("username", user.UserName);
            doc.Add("email", user.Email);
            Document timelines = new Document();
            foreach (MTimelineInfo tli in user.Timelines) timelines.Add(tli.TimelineId, TimelineInfo2DynamoDoc(tli));
            doc.Add("timelines", timelines);
            return doc;
        }

        public static MUser DynamoDoc2User(Document doc)
        {
            var user = new MUser();
            user.UserId = doc["userid"];
            user.UserName = doc["username"];
            user.Email = doc["email"];
            var listTimelineInfo = doc["timelines"].AsDocument();

            foreach (Document infoItem in listTimelineInfo.Values) user.Timelines.Add( DynamoDoc2TimelineInfo(infoItem) );
            return user;
        }

        public static Document TimelineInfo2DynamoDoc(MTimelineInfo info)
        {
            var doc = new Document();
            doc.Add("timelineid", info.TimelineId);
            doc.Add("name", info.Name);
            doc.Add("description", info.Description);
            return doc;
        }

        public static MTimelineInfo DynamoDoc2TimelineInfo(Document doc)
        {
            var tli = new MTimelineInfo(doc["timelineid"]);
            tli.Name = doc["name"];
            tli.Description = doc["description"];
            return tli;
        }

        public static Document TimelineEvent2DynamoDoc(MTimelineEvent tlevent)
        {
            var doc = new Document();
            doc.Add("timelineid", tlevent.TimelineId);
            doc.Add("title", tlevent.Title);
            doc.Add("description", tlevent.Description);
            doc.Add("image", tlevent.ImageBase64);
            doc.Add("url", tlevent.URL);
            doc.Add("data", tlevent.Data);
            doc.Add("startdate", tlevent.StartDate.ToString());
            doc.Add("enddate", tlevent.EndDate.ToString());
            doc.Add("precision", tlevent.Precision.ToString());
            return doc;
        }

        public static MTimelineEvent DynamoDoc2TimelineEvent(Document doc)
        {
            var tlevent = new MTimelineEvent();
            tlevent.TimelineId = doc["timelineid"];
            tlevent.Title = doc["title"];
            tlevent.Description = doc["description"];
            tlevent.ImageBase64 = doc["image"];
            tlevent.Image = ImageFromBase64(tlevent.ImageBase64);            
            tlevent.URL = doc["url"];
            tlevent.Data = doc["data"];
            tlevent.StartDate = Timeline.TimelineDateTime.FromTicks(Int64.Parse(doc["startdate"]));
            tlevent.EndDate = Timeline.TimelineDateTime.FromTicks(Int64.Parse(doc["enddate"]));
            tlevent.Precision = byte.Parse(doc["precision"]);
            return tlevent;
        }

        private static Xamarin.Forms.Image ImageFromBase64(string base64picture)
        {
            byte[] imageBytes = Convert.FromBase64String(base64picture);
            return new Xamarin.Forms.Image { Source = Xamarin.Forms.ImageSource.FromStream(() => new System.IO.MemoryStream(imageBytes)) };
        }

        private static DynamoDBEntry GetDDBEntry(object value)
        {
            if (value == null) return new DynamoDBNull();
            return (DynamoDBEntry)value;
        }
    }
}
