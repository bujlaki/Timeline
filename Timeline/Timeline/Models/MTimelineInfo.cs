using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Timeline.Objects.Collection;

namespace Timeline.Models
{
    public class MTimelineInfo
    {
        public string TimelineId { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Shared { get; set; }
        public string OwnerID { get; set; }
        public string OwnerName { get; set; }
        public string[] Tags { get; set; }
        //public Dictionary<string, Xamarin.Forms.Color> EventTypes { get; set; }
        public CustomObservableCollection<MEventType> EventTypes { get; set; }

        public MTimelineInfo()
        {
            TimelineId = Guid.NewGuid().ToString();
            Name = "";
            Description = "";
            Shared = false;
            Tags = new string[] { };
            //EventTypes = new Dictionary<string, Xamarin.Forms.Color>();
            //EventTypes.Add("Default", (Xamarin.Forms.Color)App.Current.Resources["bkgColor1"]);
            EventTypes = new CustomObservableCollection<MEventType>();
            EventTypes.Add(new MEventType("Default", (Xamarin.Forms.Color)App.Current.Resources["bkgColor1"]));
        }

        public MTimelineInfo(string id) : this()
        {
            TimelineId = id;
        }

        public MTimelineInfo(string name, string description) : this()
        {
            Name = name;
            Description = description;
        }

        //copy object, except the ID
        public MTimelineInfo Copy()
        {
            MTimelineInfo target = new MTimelineInfo();
            target.Name = Name;
            target.Description = Description;
            target.Shared = Shared;
            target.OwnerID = OwnerID;
            target.OwnerName = OwnerName;
            target.Tags = Tags;

            target.EventTypes.Clear();
            //IDictionaryEnumerator dictionaryEnumerator = EventTypes.GetEnumerator();
            //dictionaryEnumerator.Reset();
            //while (dictionaryEnumerator.MoveNext()) target.EventTypes.Add((string)dictionaryEnumerator.Key, (Xamarin.Forms.Color)dictionaryEnumerator.Value);
            foreach (MEventType etype in EventTypes) target.EventTypes.Add(etype);

            return target;
        }

        public void UpdateFrom(MTimelineInfo tlinfo)
        {
            Name = tlinfo.Name;
            Description = tlinfo.Description;
            Shared = tlinfo.Shared;
            OwnerID = tlinfo.OwnerID;
            OwnerName = tlinfo.OwnerName;
            Tags = tlinfo.Tags;

            EventTypes.Clear();
            foreach (MEventType etype in tlinfo.EventTypes) EventTypes.Add(etype);
            //IDictionaryEnumerator dictionaryEnumerator = tlinfo.EventTypes.GetEnumerator();
            //dictionaryEnumerator.Reset();
            //while (dictionaryEnumerator.MoveNext()) EventTypes.Add((string)dictionaryEnumerator.Key, (Xamarin.Forms.Color)dictionaryEnumerator.Value);
        }
    }
}
