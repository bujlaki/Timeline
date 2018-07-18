using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Timeline.ViewModels.Base;
using Timeline.Views;
using Timeline.Views.TestPages;
using Timeline.Models;

namespace Timeline.Services
{
	public class NavigationService : INavigationService
    {
        public INavigation _navigation => Application.Current.MainPage.Navigation;
        private VMLocator _vmLocator;
        
		//TEST
		private Lazy<VTestPage> testView;

        //REAL
        private Lazy<VLogin> loginView;
        private Lazy<VSignup> signupView;
        private Lazy<VUserPages> userpagesView;
        private Lazy<VTimelineList> timelinelistView;
        private Lazy<VOptions> optionsView;
        private Lazy<VTimeline> timelineView;
        private Lazy<VNewTimeline> newtimelineView;

        public NavigationService(VMLocator loc)
        {
            _vmLocator = loc;
            
			//TEST
			testView = new Lazy<VTestPage>(() => new VTestPage());

            //REAL
            loginView = new Lazy<VLogin>(() => new VLogin());
            signupView = new Lazy<VSignup>(() => new VSignup());
            userpagesView = new Lazy<VUserPages>(() => new VUserPages());
            timelinelistView = new Lazy<VTimelineList>(() => new VTimelineList());
            optionsView = new Lazy<VOptions>(() => new VOptions());
            timelineView = new Lazy<VTimeline>(() => new VTimeline());
            newtimelineView = new Lazy<VNewTimeline>(() => new VNewTimeline());
        }

		public Page LoginPage()
		{
            return loginView.Value;
		}

        public void ClearStackBelow(Page belowPage)
        {
            var existingPages = _navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                if(page!=belowPage) _navigation.RemovePage(page);
            }
        }

        public void ClearModalStackBelow(Page belowPage)
        {
            var existingPages = _navigation.ModalStack.ToList();
            foreach (var page in existingPages)
            {
                if (page != belowPage) _navigation.RemovePage(page);
            }
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
			_navigation.PushAsync(testView.Value);	
		}

        public void GoToLoginPage()
        {
            Application.Current.MainPage = new NavigationPage(loginView.Value);
            _navigation.PopToRootAsync();
        }

        public void GoToSignupPage()
        {
            _navigation.PushModalAsync(signupView.Value);
        }

        public void GoToUserPagesPage(MUser _user, bool clearStack = false)
        {
            //set the user
            _vmLocator.UserPagesViewModel.User = _user;

            if(clearStack)
            {
                Application.Current.MainPage = new NavigationPage(userpagesView.Value);
                _navigation.PopToRootAsync();
            }
            else
            {
                _navigation.PushModalAsync(userpagesView.Value);
            }
        }

        public void GoToTimelineView(Models.MTimelineInfo timeline)
        {
            //_vMLocator.DetailMovieViewModel.Film = itemMovie;
			_navigation.PushAsync(timelineView.Value);
        }

        public void GoToNewTimelineView()
        {
            _navigation.PushModalAsync(newtimelineView.Value);
        }

        public void GoBack() => _navigation.PopModalAsync();
    }

}
