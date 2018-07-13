using System;
using SkiaSharp;

namespace Timeline.Objects.TouchTracking
{
    class TouchInfo
    {
        private static int lastId = 0;
        private static object Lock = new object();

        public int TouchId { get; private set; }
        public DateTime InitialTime { get; set; }
        public SKPoint InitialRawPoint { get; set; }
        public SKPoint InitialPoint { get; set; }
        public SKPoint PreviousPoint { get; set; }
        public SKPoint NewPoint { get; set; }

        public TouchInfo()
        {
            lock (Lock)
            {
                TouchInfo.lastId++;
                if (lastId > 100) lastId = 1;
                TouchId = lastId;
            }
        }
}

    class LongTapData
    {
        public SKPoint InitialRawPoint { get; set; }
        public SKPoint InitialPoint { get; set; }
    }
}
