using System;
using SkiaSharp;

namespace TouchTracking
{
    class TouchInfo
    {
        public SKPoint PreviousPoint { set; get; }
        public SKPoint NewPoint { set; get; }
    }
}
