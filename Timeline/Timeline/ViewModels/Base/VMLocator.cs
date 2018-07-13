using System;

namespace Timeline.ViewModels.Base
{
    public class VMLocator
    {
		//TEST
		private Lazy<TestViewModels.VMTestPage> testViewModel;

        //REAL
        private Lazy<VMLogin> loginViewModel;
        private Lazy<VMSignup> signupViewModel;
        private Lazy<VMUserPages> userpagesViewModel;
		private Lazy<VMTimeline> timelineViewModel;
        private Lazy<VMNewTimeline> newtimelineViewModel;
        
        public VMLocator()
        {
            testViewModel = new Lazy<TestViewModels.VMTestPage>(() => new TestViewModels.VMTestPage());

            loginViewModel = new Lazy<VMLogin>(() => new VMLogin());
            signupViewModel = new Lazy<VMSignup>(() => new VMSignup());
            userpagesViewModel = new Lazy<VMUserPages>(() => new VMUserPages());
            timelineViewModel = new Lazy<VMTimeline>(() => new VMTimeline());
            newtimelineViewModel = new Lazy<VMNewTimeline>(() => new VMNewTimeline());
        }
        
		public TestViewModels.VMTestPage TestViewModel {
			get { return testViewModel.Value; }
		}

        public VMLogin LoginViewModel {
            get { return loginViewModel.Value; }
        }

        public VMSignup SignupViewModel {
            get { return signupViewModel.Value; }
        }

        public VMUserPages UserPagesViewModel {
            get { return userpagesViewModel.Value; }
        }

		public VMTimeline TimelineViewModel {
			get { return timelineViewModel.Value; }
        }

        public VMNewTimeline NewTimelineViewModel {
            get { return newtimelineViewModel.Value; }
        }
    }
}
