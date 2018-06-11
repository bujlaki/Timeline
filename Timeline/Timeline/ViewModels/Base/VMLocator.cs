using System;

using Xamarin.Forms;

using Timeline.Services;
using Timeline.Services.Base;
using Timeline.ViewModels;

namespace Timeline.ViewModels.Base
{
    public class VMLocator
    {
		//TEST
		private Lazy<TestViewModels.VMTestPage> _testViewModel;

        //REAL
        private Lazy<VMLogin> _loginViewModel;
        private Lazy<VMMainPage> _mainPageViewModel;
		private Lazy<VMTimeline> _timelineViewModel;
        
        private ServiceContainer _services;

		public ServiceContainer Services { get { return _services; } }

        public VMLocator()
        {
            _services = new ServiceContainer();
            _services.Navigation = new NavigationService(this);
            _services.Authentication = DependencyService.Get<IAuthenticationService>();

			_testViewModel = new Lazy<TestViewModels.VMTestPage>(() => new TestViewModels.VMTestPage(_services));

            _loginViewModel = new Lazy<VMLogin>(() => new VMLogin(_services));
            _mainPageViewModel = new Lazy<VMMainPage>(() => new VMMainPage(_services));
			_timelineViewModel = new Lazy<VMTimeline>(() => new VMTimeline(_services));
        }
        
		public TestViewModels.VMTestPage TestViewModel {
			get { return _testViewModel.Value; }
		}

        public VMLogin LoginViewModel {
            get { return _loginViewModel.Value; }
        }

        public VMMainPage MainPageViewModel {
            get { return _mainPageViewModel.Value; }
        }

		public VMTimeline TimelineViewModel {
			get { return _timelineViewModel.Value; }
        }
    }
}
