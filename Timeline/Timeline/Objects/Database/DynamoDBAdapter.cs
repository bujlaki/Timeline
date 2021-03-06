﻿using System;
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

            DynamoDBList favorites = new DynamoDBList();
            foreach (string fav in user.Favorites) favorites.Add(fav);
            doc.Add("favorites", favorites);

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

            var listFavorites = doc.ContainsKey("favorites") ? doc["favorites"].AsDynamoDBList() : null;
            if (listFavorites != null)
                foreach (DynamoDBEntry entry in listFavorites.Entries) user.Favorites.Add(entry.AsString());

            return user;
        }

        public static Document TimelineInfo2DynamoDoc(MTimelineInfo info)
        {
            if (info == null) return null;

            var doc = new Document();
            doc.Add("timelineid", info.TimelineId);
            doc.Add("name", info.Name);
            doc.Add("description", info.Description);

            doc.Add("shared", info.Shared.ToString().ToLower());
            if(info.OwnerID!=null) doc.Add("ownerid", info.OwnerID);
            if(info.OwnerName!=null) doc.Add("ownername", info.OwnerName);

            DynamoDBList tags = new DynamoDBList();
            foreach (string tag in info.Tags) { tags.Add(tag); }
            doc.Add("tags", tags);

            Document eventtypesDoc = new Document();
            foreach(MEventType etype in info.EventTypes)
            {
                Document etypeDoc = new Document();
                etypeDoc.Add("name", etype.TypeName);
                etypeDoc.Add("color", ColorToHEX(etype.Color));
                eventtypesDoc.Add(etype.TypeName, etypeDoc);
            }
            doc.Add("eventtypes", eventtypesDoc);
            return doc;
        }

        public static MTimelineInfo DynamoDoc2TimelineInfo(Document doc)
        {
            if (doc == null) return null;

            var tli = new MTimelineInfo(doc["timelineid"]);
            tli.Name = doc.ContainsKey("name") ? doc["name"].AsString() : "";
            tli.Description = doc.ContainsKey("description") ? doc["description"].AsString() : "";
            tli.Shared = doc.ContainsKey("shared") ? (doc["shared"].AsString().Equals("true") ? true : false) : false;
            tli.OwnerID = doc.ContainsKey("ownerid") ? doc["ownerid"].AsString() : "0";
            tli.OwnerName = doc.ContainsKey("ownername") ? doc["ownername"].AsString() : "";

            DynamoDBList tags = doc.ContainsKey("tags") ? doc["tags"].AsDynamoDBList() : null;
            if (tags != null) tli.Tags = tags.AsArrayOfString();

            var eventTypesDoc = doc.ContainsKey("eventtypes") ? doc["eventtypes"].AsDocument() : null;
            if (eventTypesDoc != null)
            {
                tli.EventTypes.Clear();
                foreach (Document item in eventTypesDoc.Values) tli.EventTypes.Add(new MEventType(item["name"], Xamarin.Forms.Color.FromHex(item["color"])));
            }

            return tli;
        }

        public static Document TimelineEvent2DynamoDoc(MTimelineEvent tlevent)
        {
            if (tlevent == null) return null;

            var doc = new Document();
            doc.Add("timelineid", tlevent.TimelineId);
            doc.Add("eventid", tlevent.EventId);
            doc.Add("title", tlevent.Title);
            if (!string.IsNullOrEmpty(tlevent.Description)) doc.Add("description", tlevent.Description);
            if (!string.IsNullOrEmpty(tlevent.ImageBase64)) doc.Add("image", tlevent.ImageBase64);
            if (!string.IsNullOrEmpty(tlevent.URL)) doc.Add("url", tlevent.URL);
            if (!string.IsNullOrEmpty(tlevent.Data)) doc.Add("data", tlevent.Data);
            doc.Add("startdate", tlevent.StartDate.Ticks);
            if (tlevent.EndDate!=null) doc.Add("enddate", tlevent.EndDate.Ticks);
            doc.Add("enddateset", tlevent.EndDateSet ? "1" : "0");
            doc.Add("precision", (byte)tlevent.StartDate.Precision);
            doc.Add("eventtype", tlevent.EventType);
            return doc;
        }

        public static MTimelineEvent DynamoDoc2TimelineEvent(Document doc)
        {
            if (doc == null) return null;

            var tlevent = new MTimelineEvent();
            tlevent.TimelineId = doc.ContainsKey("timelineid") ? doc["timelineid"].AsString() : "";
            tlevent.EventId = doc.ContainsKey("eventid") ? doc["eventid"].AsString() : "";
            tlevent.Title = doc.ContainsKey("title") ? doc["title"].AsString() : "";
            tlevent.Description = doc.ContainsKey("description") ? doc["description"].AsString() : "";
            tlevent.ImageBase64 = doc.ContainsKey("image") ? doc["image"].AsString() : "";
            tlevent.Image = tlevent.ImageBase64.Length>0 ? ImageFromBase64(tlevent.ImageBase64) : null;
            tlevent.URL = doc.ContainsKey("url") ? doc["url"].AsString() : "";
            tlevent.Data = doc.ContainsKey("data") ? doc["data"].AsString() : "";
            tlevent.StartDate = TimelineDateTime.FromTicks(Int64.Parse(doc["startdate"].AsString()));
            tlevent.EndDate = doc.ContainsKey("enddate") ? Timeline.TimelineDateTime.FromTicks(Int64.Parse(doc["enddate"].AsString())) : null;
            tlevent.EndDateSet = doc.ContainsKey("enddateset") ? (doc["enddateset"].AsString() == "1") : false;
            tlevent.Precision = byte.Parse(doc["precision"].AsString());
            tlevent.StartDate.Precision = (TimelineUnits)tlevent.Precision;
            tlevent.EndDate.Precision = (TimelineUnits)tlevent.Precision;
            tlevent.EventType = doc.ContainsKey("eventtype") ? doc["eventtype"].AsString() : "Default";
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

        private static String ColorToHEX(Xamarin.Forms.Color c)
        {
            var red = (int)(c.R * 255);
            var green = (int)(c.G * 255);
            var blue = (int)(c.B * 255);
            var hex = $"#{red:X2}{green:X2}{blue:X2}";
            return hex;
        }

    }
}
