using System;
using System.Threading.Tasks;
using Timeline.Models;
using Xamarin.Forms;

namespace Timeline.Services
{
    public interface INavigationService
    {
		Page RootPage();
        void ClearStackBelow(Page belowPage);
        void ClearModalStackBelow(Page belowPage);
        Page UserPagesView();
        Page TimelineListView();
        Page OptionsView();
        void GoToLoginPage();
        void GoToSignupPage();
        void GoToUserPagesPage(MUser _user, bool clearStack = false);
        void GoToTestPage();
		void GoToTimelineView(Models.MTimeline timeline);
		void GoBack();
    }
}
