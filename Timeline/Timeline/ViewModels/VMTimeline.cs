using System;

using Timeline.Models;

namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
		public MTimeline Timeline { get; set; }

		public VMTimeline(Services.Base.ServiceContainer services) : base(services)
        {
			Timeline = new MTimeline();
			Timeline.Events.Add(new MTimelineEvent("event1", DateTime.UtcNow));

        }
    }
}
