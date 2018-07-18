using System;
using System.Collections.Generic;
using System.Text;

using Timeline.Models;
using Xamarin.Forms;

namespace Timeline.ViewModels
{
    public class VMNewTimeline : Base.VMBase
    {
        public Command CmdCreate { get; set; }
        public MTimelineInfo Timeline { get; set; }

        public VMNewTimeline() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            Timeline = new MTimelineInfo();
        }

        private void CmdCreateExecute(object obj)
        {
            
        }
    }
}
