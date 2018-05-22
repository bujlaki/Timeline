using System;

using Xamarin.Forms;

using Timeline.Models;

namespace Timeline.ViewModels.TestViewModels
{
	public class VMTestPage : Timeline.ViewModels.Base.VMBase
    {
		public Command CMDRunTests { get; set; }

		public VMTestPage(Services.Base.ServiceContainer services) : base(services)
        {
			CMDRunTests = new Command(CMDRunTestsExecute, CMDRunTestsCanExecute);
        }

		void CMDRunTestsExecute(object obj)
        {
			PerformTests();
        }

		bool CMDRunTestsCanExecute(object arg)
        {
            return true;
        }

        private void PerformTests()
		{
			Console.WriteLine("Starting TESTS for 'MTimelineDate'");
			MTimelineDate d1 = new MTimelineDate(1);
		}
    }
}
