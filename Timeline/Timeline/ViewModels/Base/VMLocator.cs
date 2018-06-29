using System;

using Xamarin.Forms;

using Timeline.Services;
using Timeline.Services.Base;
using Timeline.Objects.Auth.Google;

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
        
        private ServiceContainer _services;

		public ServiceContainer Services { get { return _services; } }

        public VMLocator()
        {
            _services = new ServiceContainer();
            _services.Navigation = new NavigationService(this);
            _services.Database = new DBService();

            //XAML Preview doesn't work if this is directly created
            if (!DesignMode.IsDesignModeEnabled) _services.Authentication = new AuthenticationService();

            testViewModel = new Lazy<TestViewModels.VMTestPage>(() => new TestViewModels.VMTestPage(_services));

            loginViewModel = new Lazy<VMLogin>(() => new VMLogin(_services));
            signupViewModel = new Lazy<VMSignup>(() => new VMSignup(_services));
            userpagesViewModel = new Lazy<VMUserPages>(() => new VMUserPages(_services));
            timelineViewModel = new Lazy<VMTimeline>(() => new VMTimeline(_services));
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
    }
}
