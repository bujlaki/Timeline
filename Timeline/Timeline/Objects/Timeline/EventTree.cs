using System;
using System.Collections.Generic;
using System.Text;

using Timeline.Models;

namespace Timeline.Objects.Timeline
{
    public class EventTree
    {
        public TimelineUnits precision;
        public Dictionary<int, EventTree> children;
        public List<MTimelineEvent> items;

        public EventTree(TimelineUnits _precision)
        {
            precision = _precision;
            children = new Dictionary<int, EventTree>();
            items = new List<MTimelineEvent>();
        }
    }
}
