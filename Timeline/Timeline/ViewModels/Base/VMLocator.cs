using System;

using Timeline.Services;
using Timeline.Services.Base;
using Timeline.ViewModels;

namespace Timeline.ViewModels.Base
{
    public class VMLocator
    {
        private Lazy<VMMainPage> _mainPageViewModel;

        private ServiceContainer _services;

        public VMLocator()
        {
            _services = new ServiceContainer();
            _services.Navigation = new NavigationService();

            _mainPageViewModel = new Lazy<VMMainPage>(() => new VMMainPage(_services));
        }

        public VMMainPage MainPageViewModel
        {
            get { return _mainPageViewModel.Value; }
        }

    }
}
