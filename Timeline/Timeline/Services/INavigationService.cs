using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Timeline.Services
{
    public interface INavigationService
    {
		Page RootPage();
        void GoToSignupPage();
        void GoToTestPage();
		void GoToTimelineView(Models.MTimeline timeline);
		void GoBack();
    }
}
