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
			MTimelineDate tld = new MTimelineDate(2018, 1);
			Timeline.Events.Add(new MTimelineEvent("event1", tld));

        }
    }
}
