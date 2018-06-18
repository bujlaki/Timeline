using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Timeline.Services
{
    public interface INavigationService
    {
		Page RootPage(bool isLoggedIn = false);
        Page UserPagesView();
        Page TimelineListView();
        Page OptionsView();
        void GoToSignupPage();
        void GoToUserPagesPage();
        void GoToTestPage();
		void GoToTimelineView(Models.MTimeline timeline);
		void GoBack();
    }
}
