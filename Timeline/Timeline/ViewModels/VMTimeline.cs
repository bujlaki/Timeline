using System;

using Timeline.Models;

namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
		MTimeline _timeline;

		public VMTimeline(Services.Base.ServiceContainer services) : base(services)
        {
			_timeline = new MTimeline();
			_timeline.Events.Add(new MTimelineEvent("event1", DateTime.UtcNow));

        }
    }
}
