using System;
using SkiaSharp;

namespace Timeline.Objects.TouchTracking
{
    class TouchInfo
    {
        public DateTime InitialTime { get; set; }
        public SKPoint InitialPoint { get; set; }
        public SKPoint PreviousPoint { get; set; }
        public SKPoint NewPoint { get; set; }
    }
}
