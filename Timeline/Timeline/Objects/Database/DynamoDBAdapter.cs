using System;
using System.Collections.Generic;
using System.Text;

using Amazon.DynamoDBv2.DocumentModel;
using Timeline.Models;
using Timeline.Objects.Timeline;

namespace Timeline.Objects.Database
{
    class DynamoDBAdapter
    {
        public static Document User2DynamoDoc(MUser user)
        {
            if (user == null) return null;

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
            if (doc == null) return null;

            var user = new MUser();
            user.UserId = doc["userid"].AsString();
            user.UserName = doc.ContainsKey("username") ? doc["username"].AsString() : "";
            user.Email = doc.ContainsKey("email") ? doc["email"].AsString() : "";
            var listTimelineInfo = doc.ContainsKey("timelines") ? doc["timelines"].AsDocument() : null;

            if (listTimelineInfo != null)
                foreach (Document infoItem in listTimelineInfo.Values) user.Timelines.Add( DynamoDoc2TimelineInfo(infoItem) );

            return user;
        }

        public static Document TimelineInfo2DynamoDoc(MTimelineInfo info)
        {
            if (info == null) return null;

            var doc = new Document();
            doc.Add("timelineid", info.TimelineId);
            doc.Add("name", info.Name);
            doc.Add("description", info.Description);
            return doc;
        }

        public static MTimelineInfo DynamoDoc2TimelineInfo(Document doc)
        {
            if (doc == null) return null;

            var tli = new MTimelineInfo(doc["timelineid"]);
            tli.Name = doc.ContainsKey("name") ? doc["name"].AsString() : "";
            tli.Description = doc.ContainsKey("description") ? doc["description"].AsString() : ""; 
            return tli;
        }

        public static Document TimelineEvent2DynamoDoc(MTimelineEvent tlevent)
        {
            if (tlevent == null) return null;

            var doc = new Document();
            doc.Add("timelineid", tlevent.TimelineId);
            doc.Add("title", tlevent.Title);
            if (!string.IsNullOrEmpty(tlevent.Description)) doc.Add("description", tlevent.Description);
            if (!string.IsNullOrEmpty(tlevent.ImageBase64)) doc.Add("image", tlevent.ImageBase64);
            if (!string.IsNullOrEmpty(tlevent.URL)) doc.Add("url", tlevent.URL);
            if (!string.IsNullOrEmpty(tlevent.Data)) doc.Add("data", tlevent.Data);
            doc.Add("startdate", tlevent.StartDate.Ticks);
            if (tlevent.EndDate!=null) doc.Add("enddate", tlevent.EndDate.Ticks);
            doc.Add("precision", (byte)tlevent.StartDate.Precision);
            return doc;
        }

        public static MTimelineEvent DynamoDoc2TimelineEvent(Document doc)
        {
            if (doc == null) return null;

            var tlevent = new MTimelineEvent();
            tlevent.TimelineId = doc.ContainsKey("timelineid") ? doc["timelineid"].AsString() : "";
            tlevent.Title = doc.ContainsKey("title") ? doc["title"].AsString() : "";
            tlevent.Description = doc.ContainsKey("description") ? doc["description"].AsString() : "";
            tlevent.ImageBase64 = doc.ContainsKey("image") ? doc["image"].AsString() : "";
            tlevent.Image = tlevent.ImageBase64.Length>0 ? ImageFromBase64(tlevent.ImageBase64) : null;
            tlevent.URL = doc.ContainsKey("url") ? doc["url"].AsString() : "";
            tlevent.Data = doc.ContainsKey("data") ? doc["data"].AsString() : "";
            tlevent.StartDate = TimelineDateTime.FromTicks(Int64.Parse(doc["startdate"].AsString()));
            tlevent.EndDate = doc.ContainsKey("enddate") ? Timeline.TimelineDateTime.FromTicks(Int64.Parse(doc["enddate"].AsString())) : null;
            tlevent.Precision = byte.Parse(doc["precision"].AsString());
            tlevent.StartDate.Precision = (TimelineUnits)tlevent.Precision;
            tlevent.EndDate.Precision = (TimelineUnits)tlevent.Precision;
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
