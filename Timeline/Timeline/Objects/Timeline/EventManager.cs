using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Timeline.Models;

namespace Timeline.Objects.Timeline
{
    class EventManager
    {
        private static TimelineDateTime[] laneBusyUntil;

        public static MTimelineEvent GetEventAt(ObservableCollection<MTimelineEvent> events, int lane, Int64 ticks)
        {
            foreach (MTimelineEvent e in events)
            {
                if (e.LaneNumber != lane) continue;
                if (e.StartDateTicks > ticks) continue;
                if (e.EndDateTicks < ticks) continue;
                return e;
            }
            return null;
        }

        public static int SortEventsToLanes(ObservableCollection<MTimelineEvent> events, int laneCount)
        {
            int maxlane = 0;
            laneBusyUntil = new TimelineDateTime[laneCount];
            for (int i = 0; i < laneCount; i++) laneBusyUntil[i] = null;

            foreach (MTimelineEvent e in events)
            {
                e.LaneNumber = GetFreeLane(e.StartDate);
                SetLaneBusy(e.LaneNumber, e.EndDate);

                if (e.LaneNumber > maxlane) maxlane = e.LaneNumber;
            }

            return maxlane;
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
