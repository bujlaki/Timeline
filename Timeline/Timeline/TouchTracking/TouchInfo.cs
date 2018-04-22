using System;
using SkiaSharp;

namespace TouchTracking
{
    class TouchInfo
    {
        public DateTime InitialTime { get; set; }
        public SKPoint InitialPoint { get; set; }
        public SKPoint PreviousPoint { get; set; }
        public SKPoint NewPoint { get; set; }
    }
}
