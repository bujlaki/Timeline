using System;
using System.Collections.Generic;
using System.Text;

using Timeline.Models;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Collections;

namespace Timeline.ViewModels
{
    public class VMTimelineInfo : Base.VMBase
    {
        private MTimelineInfo model;
        public bool NewTimeline { get; set; }
        public MTimelineInfo TimelineInfo { get; set; }

        //commands
        public Command CmdCreate { get; set; }
        public Command CmdAddEventType { get; set; }
        public Command CmdEditEventType { get; set; }
        public Command CmdDeleteEventType { get; set; }

        public VMTimelineInfo() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            CmdAddEventType = new Command(CmdAddEventTypeExecute);
            CmdEditEventType = new Command(CmdEditEventTypeExecute);
            CmdDeleteEventType = new Command(CmdDeleteEventTypeExecute);

            TimelineInfo = new MTimelineInfo();
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
            UpdateAllProperties();
        }

        private void CmdAddEventTypeExecute(object obj)
        {
            DictionaryEntry entry = new DictionaryEntry();
            entry.Key = "";
            entry.Value = Color.Black;
            App.services.Navigation.GoToEventTypeView(entry);
        }

        private void CmdEditEventTypeExecute(object obj)
        {
            App.services.Navigation.GoToEventTypeView((DictionaryEntry)obj);
        }

        private void CmdDeleteEventTypeExecute(object obj)
        {
            App.services.Navigation.GoBack();
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
    }
}
