using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Timeline.Models;

namespace Timeline.ViewModels
{
    public class VMEventType : Base.VMBase
    {
        private DictionaryEntry model;

        public DictionaryEntry EventType;
        public bool NewEventType { get; set; }

        public string TypeName
        {
            get { return (string)EventType.Key; }
            set { EventType.Key = value; }
        }

        public Xamarin.Forms.Color TypeColor
        {
            get { return (Xamarin.Forms.Color)EventType.Value; }
            set { EventType.Value = value; }
        }

        public VMEventType() : base()
        {

        }

        public void SetModel(DictionaryEntry etype)
        {
            if(etype.Key as string == "") {
                model.Key = "";
                model.Value = Xamarin.Forms.Color.Black;
                NewEventType = true;
            }
            else {
                model = etype;
                NewEventType = false;
            }
            EventType = model;
            UpdateAllProperties();
        }
    }
}
