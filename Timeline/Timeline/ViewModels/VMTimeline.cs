using System;

using Timeline.Models;
using Timeline.Objects.Date;

namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
		public MTimeline Timeline { get; set; }

		public VMTimeline(Services.Base.ServiceContainer services) : base(services)
        {
			Timeline = new MTimeline();
            //TimelineDateTime tld = new TimelineDateTime(2018, 1);
			for (int i = 1; i < 11; i++)
			{
				Timeline.Events.Add(new MTimelineEvent("event1", new TimelineDateTime(2018, i), 2));

			}
        }
    }
}
