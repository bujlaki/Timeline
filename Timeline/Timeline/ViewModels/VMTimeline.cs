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
			for (int i = 0; i < 10; i++)
			{
				Timeline.Events.Add(new MTimelineEvent("event1", tld, 2));
				tld.Add();
			}
        }
    }
}
