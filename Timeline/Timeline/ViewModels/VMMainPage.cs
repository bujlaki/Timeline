using System;
using Xamarin.Forms;

namespace Timeline.ViewModels
{
    public class VMMainPage : Base.VMBase
    {
		Command CMDOpenTimeline;

        public VMMainPage(Services.Base.ServiceContainer services) : base(services)
        {
			CMDOpenTimeline = new Command(CMDOpenTimelineExecute, CMDOpenTimelineCanExecute);
        }

		void CMDOpenTimelineExecute(object obj)
		{
			_services.Navigation.
		}

		bool CMDOpenTimelineCanExecute(object arg)
		{
			return true;
		}

    }
}
