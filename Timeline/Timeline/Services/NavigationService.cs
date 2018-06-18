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
        private Lazy<VUserPages> userpagesView;
        private Lazy<VTimelineList> timelinelistView;
        private Lazy<VOptions> optionsView;
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
            userpagesView = new Lazy<VUserPages>(() => new VUserPages());
            timelinelistView = new Lazy<VTimelineList>(() => new VTimelineList());
            optionsView = new Lazy<VOptions>(() => new VOptions());
            mainpageView = new Lazy<VMainPage>(() => new VMainPage());
            timelineView = new Lazy<VTimeline>(() => new VTimeline());
        }

		public Page RootPage(bool isLoggedIn = false)
		{
            if (isLoggedIn) { return userpagesView.Value; }
            return loginView.Value;
		}

        public Page UserPagesView()
        {
            return userpagesView.Value;
        }

        public Page TimelineListView()
        {
            return timelinelistView.Value;
        }

        public Page OptionsView()
        {
            return optionsView.Value;
        }

        public void GoToTestPage()
		{
			_navigation.PushModalAsync(testView.Value);	
		}

        public void GoToSignupPage()
        {
            _navigation.PushModalAsync(signupView.Value);
        }

        public void GoToUserPagesPage()
        {
            _navigation.PushModalAsync(userpagesView.Value);
        }

        public void GoToTimelineView(Models.MTimeline timeline)
        {
            //_vMLocator.DetailMovieViewModel.Film = itemMovie;
			_navigation.PushModalAsync(timelineView.Value);
        }

        public void GoBack() => _navigation.PopModalAsync();
    }

}
