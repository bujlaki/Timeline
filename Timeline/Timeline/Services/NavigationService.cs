using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Essentials;

using Timeline.ViewModels.Base;
using Timeline.Views;
using Timeline.Views.TestPages;
using Timeline.Models;
using System.Collections;

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
        private Lazy<VTimelineInfo> timelineInfoView;
        private Lazy<VTimelineEvent> timelineEventView;
        private Lazy<VPictograms> pictogramsView;
        private Lazy<VEventType> eventTypeView;

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
            timelineInfoView = new Lazy<VTimelineInfo>(() => new VTimelineInfo());
            timelineEventView = new Lazy<VTimelineEvent>(() => new VTimelineEvent());
            pictogramsView = new Lazy<VPictograms>(() => new VPictograms());
            eventTypeView = new Lazy<VEventType>(() => new VEventType());
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
			_navigation.PushModalAsync(testView.Value);	
		}

        public void GoToLoginPage()
        {
            Application.Current.MainPage = new NavigationPage(loginView.Value);
            _navigation.PopToRootAsync();
        }

        public void GoToSignupPage()
        {
            _vmLocator.SignupViewModel.PolicyAccepted = false;
            _navigation.PushModalAsync(signupView.Value);
        }

        public void GoToUserPagesPage(string userid, bool clearStack = false)
        {
            //set the user
            MUser u = Task.Run(async () => await App.services.Database.GetUser(userid)).Result;
            _vmLocator.UserPagesViewModel.InitView(u);
            //_vmLocator.UserPagesViewModel.User = Task.Run(async () => await App.services.Database.GetUser(userid)).Result;
            
            if (clearStack)
            {
                MainThread.BeginInvokeOnMainThread(()=> { Application.Current.MainPage = new NavigationPage(userpagesView.Value); });
                _navigation.PopToRootAsync();
            }
            else
            {
                _navigation.PushModalAsync(userpagesView.Value);
            }
        }

        public void GoToTimelineView(MTimelineInfo timeline)
        {
            _vmLocator.TimelineViewModel.ZoomUnit = Objects.Timeline.TimelineUnits.Year;
            _vmLocator.TimelineViewModel.SetModel(timeline);
            try
            {
                _navigation.PushModalAsync(timelineView.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GoToTimelineView ERROR: " + ex.Message);
            }
        }

        public void GoToTimelineInfoView(MTimelineInfo tlinfo)
        {
            _vmLocator.TimelineInfoViewModel.SetModel(tlinfo);
            _navigation.PushModalAsync(timelineInfoView.Value);
        }

        public void GoToTimelineEventView(MTimelineEvent tlevent)
        {
            _vmLocator.TimelineEventViewModel.InitView(tlevent);
            if (string.IsNullOrEmpty(tlevent.Title))
            {
                _vmLocator.TimelineEventViewModel.PageTitle = "CREATE EVENT";
                _vmLocator.TimelineEventViewModel.IsNewEvent = true;
            }
            else
            {
                _vmLocator.TimelineEventViewModel.PageTitle = "EDIT EVENT";
                _vmLocator.TimelineEventViewModel.IsNewEvent = false;
            }

            _navigation.PushModalAsync(timelineEventView.Value);
        }

        public void GoToPictogramsView()
        {
            _vmLocator.PictogramsViewModel.LoadPictograms();
            _navigation.PushModalAsync(pictogramsView.Value);
        }

        public void GoToEventTypeView(DictionaryEntry etype)
        {
            _vmLocator.EventTypeViewModel.SetModel(etype);
            _navigation.PushModalAsync(eventTypeView.Value);
        }

        public void GoBack() => _navigation.PopModalAsync();
    }

}
