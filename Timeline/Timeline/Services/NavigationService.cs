using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Timeline.ViewModels.Base;
using Timeline.Views;
using Timeline.Views.TestPages;

namespace Timeline.Services
{
	public class NavigationService : INavigationService
    {
        public INavigation _navigation => Application.Current.MainPage.Navigation;
        private VMLocator _vMLocator;
        
		//TEST
		private Lazy<VTestPage> testView;

        //REAL
        private Lazy<VLogin> loginView;
        private Lazy<VSignup> signupView;
		private Lazy<VMainPage> mainpageView;
        private Lazy<VTimeline> timelineView;

        public NavigationService(VMLocator loc)
        {
            _vMLocator = loc;
            
			//TEST
			testView = new Lazy<VTestPage>(() => new VTestPage());

            //REAL
            loginView = new Lazy<VLogin>(() => new VLogin());
            signupView = new Lazy<VSignup>(() => new VSignup());
            mainpageView = new Lazy<VMainPage>(() => new VMainPage());
            timelineView = new Lazy<VTimeline>(() => new VTimeline());
        }

		public Page RootPage()
		{
            //return mainpageView.Value;
            return loginView.Value;
		}

        public void GoToTestPage()
		{
			_navigation.PushModalAsync(testView.Value);	
		}

        public void GoToSignupPage()
        {
            _navigation.PushModalAsync(signupView.Value);
        }

        public void GoToTimelineView(Models.MTimeline timeline)
        {
            //_vMLocator.DetailMovieViewModel.Film = itemMovie;
			_navigation.PushModalAsync(timelineView.Value);
        }

        public void GoBack() => _navigation.PopModalAsync();
    }

}
