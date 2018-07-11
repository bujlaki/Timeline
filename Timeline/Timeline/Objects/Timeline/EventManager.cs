using System;
using System.Collections.Generic;
using System.Text;

using Timeline.Models;
using Timeline.Objects.Date;

namespace Timeline.Objects.Timeline
{
    class EventManager
    {
        private static TimelineDateTime[] laneBusyUntil;

        public static void SortEventsToLanes(ref MTimeline timeline, int laneCount)
        {
            laneBusyUntil = new TimelineDateTime[laneCount];
            for (int i = 0; i < laneCount; i++) laneBusyUntil[i] = null;

            foreach (MTimelineEvent e in timeline.Events)
            {
                e.LaneNumber = GetFreeLane(e.StartDate);
                SetLaneBusy(e.LaneNumber, e.EndDate);
            }
        }

        private static void SetLaneBusy(int lane, TimelineDateTime tld)
        {
            if (laneBusyUntil[lane] == null) laneBusyUntil[lane] = new TimelineDateTime(DateTime.UtcNow);
            tld.CopyTo(ref laneBusyUntil[lane]);
        }

        private static int GetFreeLane(TimelineDateTime tld)
        {
            for (int i = 0; i < 8; i++)
                if ((laneBusyUntil[i] == null) || (laneBusyUntil[i] < tld)) return i;

            return -1;
        }
    }
}
