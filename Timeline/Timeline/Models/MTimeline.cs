using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline.Models
{
    public class MTimeline
    {
        public string Name { get; set; }
        public List<MTimelineEvent> Events { get; }
        
        public MTimeline()
        {
            Events = new List<MTimelineEvent>();
        }

    }
}
