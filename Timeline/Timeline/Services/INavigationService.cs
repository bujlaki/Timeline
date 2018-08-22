using System;
using System.Collections;
using System.Threading.Tasks;
using Timeline.Models;
using Xamarin.Forms;

namespace Timeline.Services
{
    public interface INavigationService
    {
		Page LoginPage();
        void ClearStackBelow(Page belowPage);
        void ClearModalStackBelow(Page belowPage);
        Page UserPagesView();
        Page TimelineListView();
        Page OptionsView();
        void GoToLoginPage();
        void GoToSignupPage();
        void GoToUserPagesPage(string userid, bool clearStack = false);
        void GoToTestPage();
		void GoToTimelineView(MTimelineInfo timeline);
        void GoToTimelineInfoView(MTimelineInfo tlinfo);
        void GoToTimelineEventView(MTimelineEvent tlevent);
        void GoToPictogramsView();
        void GoToEventTypeView(DictionaryEntry etype);
        void GoBack();
    }
}
