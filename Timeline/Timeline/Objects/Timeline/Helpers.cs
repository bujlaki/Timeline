﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Objects.Timeline
{
    public enum TimelineUnits
    {
        Minute,
        Hour,
        Day,
        Month,
        Year,
        Decade,
        Century,
        All
    }

    public enum TimelineOrientation
    {
        Portrait,
        Landscape
    }

    public class TapEventArg
    {
        public float X;
        public float Y;
        public int Lane;
        public Int64 Ticks;
        public TimelineUnits ZoomUnit;

        public TapEventArg(float _x, float _y, int _lane, Int64 _ticks, TimelineUnits _zoomUnit)
        {
            X = _x;
            Y = _y;
            Lane = _lane;
            Ticks = _ticks;
            ZoomUnit = _zoomUnit;
        }
    }
}
