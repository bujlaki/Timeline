using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline.Models
{
    public class MTimeline
    {
        public string TimelineId { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MTimelineEvent> Events { get; }
        public int MaxLane { get; set; }

        public MTimeline()
        {
            TimelineId = Guid.NewGuid().ToString();
            Events = new List<MTimelineEvent>();
        }

    }
}
