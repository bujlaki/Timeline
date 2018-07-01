using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Acr.UserDialogs;
using Timeline.Models;
using Timeline.Models.DynamoDBModels;


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

            MDBUser dbuser;
            _services.Database.Connect(_services.Authentication.CurrentUser.AWSCredentials);
            dbuser = Task.Run(async () => await _services.Database.GetUser("1")).Result;

            UserDialogs.Instance.Alert(dbuser.UserName);

            dbuser.Timelines.Add(new MDBTimelineInfo("1","my timeline1","just a test"));

            Task.Run(async () => await _services.Database.UpdateUser(dbuser));

            UserDialogs.Instance.Alert("done");
		}
    }
}
