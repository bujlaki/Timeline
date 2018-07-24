using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Timeline.ViewModels
{
    public class VMTimelineEvent : Base.VMBase
    {
        public Command CmdCreate { get; set; }
        public VMTimelineEvent Event { get; set; }

        public VMTimelineEvent() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
        }

        private void CmdCreateExecute(object obj)
        {

            App.services.Navigation.GoBack();
        }
    }
}
