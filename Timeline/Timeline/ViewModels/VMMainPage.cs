using System;
using Xamarin.Forms;

namespace Timeline.ViewModels
{
    public class VMMainPage : Base.VMBase
    {
		public Command CMDOpenTimeline { get; set; }
		public Command CMDOpenTest { get; set; }
        
        public VMMainPage(Services.Base.ServiceContainer services) : base(services)
        {
			CMDOpenTimeline = new Command(CMDOpenTimelineExecute, CMDOpenTimelineCanExecute);
			CMDOpenTest = new Command(CMDOpenTestExecute, CMDOpenTestCanExecute);
        }

		void CMDOpenTimelineExecute(object obj)
		{
			_services.Navigation.GoToTimelineView(null);
		}

		bool CMDOpenTimelineCanExecute(object arg)
		{
			return true;
		}

		void CMDOpenTestExecute(object obj) {
            _services.Navigation.GoToTestPage();
        }

        bool CMDOpenTestCanExecute(object arg) {
            return true;
        }
    }
}
