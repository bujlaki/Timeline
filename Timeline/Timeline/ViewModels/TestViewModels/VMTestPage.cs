using System;
using System.Threading.Tasks;

using Xamarin.Forms;

using Acr.UserDialogs;

using Timeline.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Timeline.ViewModels.TestViewModels
{
	public class VMTestPage : Timeline.ViewModels.Base.VMBase
    {
        private int pickedValue;
        private int minValue;
		public Command CmdRunTests { get; set; }
        public Command CmdOpenTimeline { get; set; }
        public ObservableCollection<object> Values { get; set; }
        public int PickedValue {
            get { return pickedValue; }
            set { pickedValue = value; RaisePropertyChanged("PickedValue"); }
        }
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; RaisePropertyChanged("MinValue"); }
        }

        public VMTestPage() : base()
        {
			CmdRunTests = new Command(CmdRunTestsExecute);
            CmdOpenTimeline = new Command(CmdOpenTimelineExecute);

            Values = new ObservableCollection<object>();
            Values.Add("January");
            Values.Add("February");
            Values.Add("March");
            Values.Add("April");
            Values.Add("May");
            Values.Add("June");
            Values.Add("July");
            Values.Add("August");
            Values.Add("September");
            Values.Add("October");
            Values.Add("November");
            Values.Add("December");
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
            //Console.WriteLine("Starting TESTS for 'MTimelineDate'");

            //         MUser user;
            //         App.services.Database.Connect(App.services.Authentication.Login.AWSCredentials);
            //         user = Task.Run(async () => await App.services.Database.GetUser("1")).Result;

            //         UserDialogs.Instance.Alert(user.UserName);

            //         user.Timelines.Add(new MTimelineInfo("my timeline1","just a test"));


            //         Task.Run(async () => await App.services.Database.UpdateUser(user));

            //         UserDialogs.Instance.Alert("done");

            MinValue = 50;
		}
    }
}
