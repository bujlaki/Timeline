using System;
using System.Collections;
using System.Collections.Generic;

namespace Timeline.Models
{
    public class MTimelineInfo
    {
        public string TimelineId { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public MTimelineInfo()
        {
            TimelineId = Guid.NewGuid().ToString();
        }

        public MTimelineInfo(string id)
        {
            TimelineId = id;
        }

        public MTimelineInfo(string name, string description)
        {
            TimelineId = Guid.NewGuid().ToString();
            Name = name;
            Description = description;
        }

    }
}
