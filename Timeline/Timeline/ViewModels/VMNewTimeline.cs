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
        public MTimeline Timeline { get; set; }

        public VMNewTimeline() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            Timeline = new MTimeline();
        }

        private void CmdCreateExecute(object obj)
        {
            
        }
    }
}
