using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using Timeline.Models;
using Timeline.Objects.Collection;
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

        public CustomObservableCollection<MEventType> EventTypes { get; set; }
        public ObservableCollection<string> Tags { get; set; }

        //tab segments
        public int SelectedSegment { get; set; }
        public bool ShowEventTypes { get { return SelectedSegment == 0; } }
        public bool ShowSearchTags { get { return SelectedSegment == 1; } }

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
        public Command CmdTabSegmentTap { get; set; }
        public Command CmdAddTag { get; set; }
        public Command CmdDeleteTag { get; set; }

        public VMTimelineInfo() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            CmdUpdate = new Command(CmdUpdateExecute);

            CmdAddEventType = new Command(CmdAddEventTypeExecute);
            CmdEditEventType = new Command(CmdEditEventTypeExecute);
            CmdDeleteEventType = new Command(CmdDeleteEventTypeExecute);

            CmdSetEventTypeColor = new Command(CmdSetEventTypeColorExecute);
            CmdCancelEventTypeColor = new Command(CmdCancelEventTypeColorExecute);

            CmdTabSegmentTap = new Command(CmdTabSegmentTapExecute);
            CmdAddTag = new Command(CmdAddTagExecute);
            CmdDeleteTag = new Command(CmdDeleteTagExecute);

            TimelineInfo = new MTimelineInfo();
            EventTypes = new CustomObservableCollection<MEventType>();
            Tags = new ObservableCollection<string>();
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
            foreach (MEventType etype in TimelineInfo.EventTypes) EventTypes.Add(etype);

            Tags.Clear();
            foreach (string tag in TimelineInfo.Tags) Tags.Add(tag);

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
            MEventType etype = TimelineInfo.EventTypes.FirstOrDefault(x => x.TypeName == key);
            if (etype==null) {
                TimelineInfo.EventTypes.Add(new MEventType(key, color));
                EventTypes.Add(new MEventType(key, color));
            }
            else {
                UserDialogs.Instance.Toast("There is already a type with that name");
            }
        }

        private void DeleteEventType(string key)
        {
            var i = -1;
            MEventType etype = TimelineInfo.EventTypes.FirstOrDefault(x => x.TypeName == key);
            if (etype==null) return;

            TimelineInfo.EventTypes.Remove(etype);

            etype = EventTypes.FirstOrDefault(x => x.TypeName == key);
            EventTypes.Remove(etype);
        }

        private void UpdateEventType(string key, Color color)
        {
            var i = -1;
            MEventType etype = TimelineInfo.EventTypes.FirstOrDefault(x => x.TypeName == key);
            if (etype == null) return;

            etype.Color = color;

            etype = EventTypes.FirstOrDefault(x => x.TypeName == key);
            etype.Color = color;

            EventTypes.ReportItemChange(etype);
        }

        private void CmdEditEventTypeExecute(object obj)
        {
            MEventType etype = (MEventType)obj;
            PickedTypeName = etype.TypeName;
            PickedColor = etype.Color;
            IsPicking = true;
        }

        private void CmdDeleteEventTypeExecute(object obj)
        {
            MEventType etype = (MEventType)obj;
            DeleteEventType(etype.TypeName);
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

        private void CmdTabSegmentTapExecute(object obj)
        {
            RaisePropertyChanged("ShowEventTypes");
            RaisePropertyChanged("ShowSearchTags");
        }

        private void CmdAddTagExecute(object obj)
        {
            PromptConfig pc = new PromptConfig { Title = "Search tag" };
            PromptResult pr;
            Task.Run(async () =>
            {
                pr = await UserDialogs.Instance.PromptAsync(pc);

                if (pr.Ok)
                {
                    if (pr.Text != "") AddSearchTag(pr.Text);
                    else UserDialogs.Instance.Toast("Invalid tag");
                }
            });
        }

        private void CmdDeleteTagExecute(object obj)
        {
            DeleteSearchTag((string)obj);
        }

        private void AddSearchTag(string tag)
        {
            if (!TimelineInfo.Tags.Contains(tag))
            {
                Tags.Add(tag);
                TimelineInfo.Tags = Tags.ToArray();
            }
            else
            {
                UserDialogs.Instance.Toast("This tag is already defined");
            }
        }

        private void DeleteSearchTag(string tag)
        {
            var i = -1;
            if (!TimelineInfo.Tags.Contains(tag)) return;

            Tags.Remove(tag);
            TimelineInfo.Tags = Tags.ToArray();
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
            MessagingCenter.Send<VMTimelineInfo, MTimelineInfo>(this, "TimelineInfo_updated", model);
            App.services.Navigation.GoBack();
        }
    }
}
