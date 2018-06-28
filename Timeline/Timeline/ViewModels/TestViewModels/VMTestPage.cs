using System;

using Xamarin.Forms;

using Timeline.Models;

namespace Timeline.ViewModels.TestViewModels
{
	public class VMTestPage : Timeline.ViewModels.Base.VMBase
    {
		public Command CmdRunTests { get; set; }
        public Command CmdOpenTimeline { get; set; }

        public VMTestPage(Services.Base.ServiceContainer services) : base(services)
        {
			CmdRunTests = new Command(CmdRunTestsExecute);
            CmdOpenTimeline = new Command(CmdOpenTimelineExecute);
        }

		void CmdRunTestsExecute(object obj)
        {
			PerformTests();
        }

        void CmdOpenTimelineExecute(object obj)
        {
            _services.Navigation.GoToTimelineView(null);
        }

        private void PerformTests()
		{
			Console.WriteLine("Starting TESTS for 'MTimelineDate'");
			MTimelineDate d1 = new MTimelineDate(1);
		}
    }
}
