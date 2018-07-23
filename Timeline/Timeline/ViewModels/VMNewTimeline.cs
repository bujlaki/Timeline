using System;
using System.Collections.Generic;
using System.Text;

using Timeline.Models;
using Xamarin.Forms;
using Acr.UserDialogs;

namespace Timeline.ViewModels
{
    public class VMNewTimeline : Base.VMBase
    {
        public Command CmdCreate { get; set; }
        public MTimelineInfo TimelineInfo { get; set; }

        public VMNewTimeline() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            TimelineInfo = new MTimelineInfo();
        }

        private void CmdCreateExecute(object obj)
        {
            if (String.IsNullOrEmpty(TimelineInfo.Name))
            {
                UserDialogs.Instance.Alert("Please set a name for the timeline", "Name not set");
                return;
            }
            if (String.IsNullOrEmpty(TimelineInfo.Description)) TimelineInfo.Description = "";

            MessagingCenter.Send<VMNewTimeline, MTimelineInfo>(this, "TimelineInfo_created", TimelineInfo);
            App.services.Navigation.GoBack();
        }
    }
}
