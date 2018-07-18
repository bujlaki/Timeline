using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Acr.UserDialogs;

using Timeline.Models;


namespace Timeline.ViewModels.TestViewModels
{
	public class VMTestPage : Timeline.ViewModels.Base.VMBase
    {
		public Command CmdRunTests { get; set; }
        public Command CmdOpenTimeline { get; set; }

        public VMTestPage() : base()
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
            App.services.Navigation.GoToTimelineView(null);
        }

        private void PerformTests()
		{
			Console.WriteLine("Starting TESTS for 'MTimelineDate'");

            MUser user;
            App.services.Database.Connect(App.services.Authentication.Login.AWSCredentials);
            user = Task.Run(async () => await App.services.Database.GetUser("1")).Result;

            UserDialogs.Instance.Alert(user.UserName);

            user.Timelines.Add(new MTimelineInfo("my timeline1","just a test"));


            Task.Run(async () => await App.services.Database.UpdateUser(user));

            UserDialogs.Instance.Alert("done");
		}
    }
}
