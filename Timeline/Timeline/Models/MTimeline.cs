using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline.Models
{
    public class MTimeline
    {
        private List<MTimelineEvent> events;

        public List<MTimelineEvent> Events
        {
            get { return events; }
        }

        public MTimeline()
        {
            events = new List<MTimelineEvent>();
        }

    }
}
