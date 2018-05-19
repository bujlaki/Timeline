using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Timeline.ViewModels.Base;
using Timeline.Views;

namespace Timeline.Services
{
	public class NavigationService : INavigationService
    {
        public INavigation _navigation => Application.Current.MainPage.Navigation;
        private VMLocator _vMLocator;

		private Lazy<VMainPage> mainpageView;
        private Lazy<VTimeline> timelineView;

        public NavigationService(VMLocator loc)
        {
            _vMLocator = loc;
			mainpageView = new Lazy<VMainPage>(() => new VMainPage());
			timelineView = new Lazy<VTimeline>(() => new VTimeline());
        }

		public Page RootPage()
		{
			return mainpageView.Value;
		}

        public void GoToTimelineView(Models.MTimeline timeline)
        {
            //_vMLocator.DetailMovieViewModel.Film = itemMovie;
			_navigation.PushModalAsync(timelineView.Value);
        }

        public void GoBack() => _navigation.PopModalAsync();
    }

}
