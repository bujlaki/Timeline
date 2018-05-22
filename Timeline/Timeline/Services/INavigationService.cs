using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Timeline.Services
{
    public interface INavigationService
    {
		Page RootPage();
		void GoToTestPage();
		void GoToTimelineView(Models.MTimeline timeline);
		void GoBack();
    }
}
