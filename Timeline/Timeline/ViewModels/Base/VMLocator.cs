﻿using System;

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
        private Lazy<VMTimelineInfo> timelineInfoViewModel;
        private Lazy<VMTimelineEvent> timelineEventViewModel;
        
        public VMLocator()
        {
            testViewModel = new Lazy<TestViewModels.VMTestPage>(() => new TestViewModels.VMTestPage());

            loginViewModel = new Lazy<VMLogin>(() => new VMLogin());
            signupViewModel = new Lazy<VMSignup>(() => new VMSignup());
            userpagesViewModel = new Lazy<VMUserPages>(() => new VMUserPages());
            timelineViewModel = new Lazy<VMTimeline>(() => new VMTimeline());
            timelineInfoViewModel = new Lazy<VMTimelineInfo>(() => new VMTimelineInfo());
            timelineEventViewModel = new Lazy<VMTimelineEvent>(() => new VMTimelineEvent());
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

        public VMTimelineInfo TimelineInfoViewModel {
            get { return timelineInfoViewModel.Value; }
        }

        public VMTimelineEvent TimelineEventViewModel {
            get { return timelineEventViewModel.Value; }
        }
    }
}
