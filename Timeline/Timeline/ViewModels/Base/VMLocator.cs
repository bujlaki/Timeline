using System;

using Timeline.Services;
using Timeline.Services.Base;
using Timeline.ViewModels;

namespace Timeline.ViewModels.Base
{
    public class VMLocator
    {
        private Lazy<VMMainPage> _mainPageViewModel;
		private Lazy<VMTimeline> _timelineViewModel;

        private ServiceContainer _services;

		public ServiceContainer Services { get { return _services; } }

        public VMLocator()
        {
            _services = new ServiceContainer();
            _services.Navigation = new NavigationService(this);

            _mainPageViewModel = new Lazy<VMMainPage>(() => new VMMainPage(_services));
			_timelineViewModel = new Lazy<VMTimeline>(() => new VMTimeline(_services));
        }

        public VMMainPage MainPageViewModel
        {
            get { return _mainPageViewModel.Value; }
        }

		public VMTimeline TimelineViewModel
        {
			get { return _timelineViewModel.Value; }
        }
    }
}
