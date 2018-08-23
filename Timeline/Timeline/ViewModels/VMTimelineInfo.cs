using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Timeline.Models;
using Acr.UserDialogs;
using Amporis.Xamarin.Forms.ColorPicker;
using System.Collections.ObjectModel;
using System.Linq;

namespace Timeline.ViewModels
{
    public class VMTimelineInfo : Base.VMBase
    {
        private MTimelineInfo model;

        public bool NewTimeline { get; set; }
        public MTimelineInfo TimelineInfo { get; set; }

        public ObservableCollection<KeyValuePair<string, Color>> EventTypes { get; set; }

        //color picking
        private bool isPicking;
        private string pickedTypeName;
        private Color pickedColor;
        public bool IsPicking { get { return isPicking; } set { isPicking = value; RaisePropertyChanged("IsPicking"); } }
        public string PickedTypeName { get { return pickedTypeName; } set { pickedTypeName = value; RaisePropertyChanged("PickedTypeName"); } }
        public Color PickedColor { get { return pickedColor; } set { pickedColor = value; RaisePropertyChanged("PickedColor"); } }

        //commands
        public Command CmdCreate { get; set; }
        public Command CmdUpdate { get; set; }
        public Command CmdAddEventType { get; set; }
        public Command CmdEditEventType { get; set; }
        public Command CmdDeleteEventType { get; set; }
        public Command CmdSetEventTypeColor { get; set; }
        public Command CmdCancelEventTypeColor { get; set; }

        public VMTimelineInfo() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            CmdUpdate = new Command(CmdUpdateExecute);

            CmdAddEventType = new Command(CmdAddEventTypeExecute);
            CmdEditEventType = new Command(CmdEditEventTypeExecute);
            CmdDeleteEventType = new Command(CmdDeleteEventTypeExecute);

            CmdSetEventTypeColor = new Command(CmdSetEventTypeColorExecute);
            CmdCancelEventTypeColor = new Command(CmdCancelEventTypeColorExecute);

            TimelineInfo = new MTimelineInfo();
            EventTypes = new ObservableCollection<KeyValuePair<string, Color>>();
        }

        public void SetModel(MTimelineInfo tlinfo)
        {
            if (tlinfo == null) {
                model = new MTimelineInfo();
                NewTimeline = true;
            }
            else {
                model = tlinfo;
                NewTimeline = false;
            }
            TimelineInfo = model.Copy();

            EventTypes.Clear();
            foreach (KeyValuePair<string, Color> kvp in TimelineInfo.EventTypes) EventTypes.Add(kvp);
            UpdateAllProperties();
        }

        private void CmdAddEventTypeExecute(object obj)
        {
            PromptConfig pc = new PromptConfig { Title = "TypeName" };
            PromptResult pr;
            Task.Run(async () =>
            {
                pr = await UserDialogs.Instance.PromptAsync(pc);

                if (pr.Ok) {
                    if (pr.Text != "") AddEventType(pr.Text, Color.White);
                    else UserDialogs.Instance.Toast("Invalid name");
                }
            });
        }

        private void AddEventType(string key, Color color)
        {
            if (!TimelineInfo.EventTypes.ContainsKey(key)) {
                TimelineInfo.EventTypes.Add(key, color);
                EventTypes.Add(new KeyValuePair<string, Color>(key, color));
            }
            else {
                UserDialogs.Instance.Toast("There is already a type with that name");
            }
        }

        private void DeleteEventType(string key)
        {
            var i = -1;
            if (!TimelineInfo.EventTypes.ContainsKey(key)) return;
            TimelineInfo.EventTypes.Remove(key);
            foreach (KeyValuePair<string, Color> kvp in EventTypes)
            {
                if (kvp.Key == key) {
                    i = EventTypes.IndexOf(kvp);
                    break;
                }
            }
            if (i >= 0) EventTypes.RemoveAt(i);
        }

        private void UpdateEventType(string key, Color color)
        {
            var i = -1;
            if (!TimelineInfo.EventTypes.ContainsKey(key)) return;
            TimelineInfo.EventTypes[key] = color;
            foreach(KeyValuePair<string,Color> kvp in EventTypes)
            {
                if(kvp.Key == key) {
                    i = EventTypes.IndexOf(kvp);
                    break;
                }
            }
            if (i >= 0)
            {
                EventTypes.RemoveAt(i);
                EventTypes.Insert(i, new KeyValuePair<string, Color>(key, color));
            }
        }

        private void CmdEditEventTypeExecute(object obj)
        {
            KeyValuePair<string, Color> entry = (KeyValuePair<string, Color>)obj;
            PickedTypeName = (string)entry.Key;
            PickedColor = (Color)entry.Value;
            IsPicking = true;
        }

        private void CmdDeleteEventTypeExecute(object obj)
        {
            KeyValuePair<string, Color> entry = (KeyValuePair<string, Color>)obj;
            DeleteEventType(entry.Key);
        }

        private void CmdSetEventTypeColorExecute(object obj)
        {
            IsPicking = false;
            UpdateEventType(PickedTypeName, PickedColor);
        }

        private void CmdCancelEventTypeColorExecute(object obj)
        {
            IsPicking = false;
        }

        private void CmdCreateExecute(object obj)
        {
            if (String.IsNullOrEmpty(TimelineInfo.Name))
            {
                UserDialogs.Instance.Alert("Please set a name for the timeline", "Name not set");
                return;
            }
            if (String.IsNullOrEmpty(TimelineInfo.Description)) TimelineInfo.Description = "";

            MessagingCenter.Send<VMTimelineInfo, MTimelineInfo>(this, "TimelineInfo_created", TimelineInfo);
            App.services.Navigation.GoBack();
        }

        private void CmdUpdateExecute(object obj)
        {
            if (String.IsNullOrEmpty(TimelineInfo.Name))
            {
                UserDialogs.Instance.Alert("Please set a name for the timeline", "Name not set");
                return;
            }
            if (String.IsNullOrEmpty(TimelineInfo.Description)) TimelineInfo.Description = "";

            model.UpdateFrom(TimelineInfo);
            MessagingCenter.Send<VMTimelineInfo, MTimelineInfo>(this, "TimelineInfo_updated", TimelineInfo);
            App.services.Navigation.GoBack();
        }
    }
}
