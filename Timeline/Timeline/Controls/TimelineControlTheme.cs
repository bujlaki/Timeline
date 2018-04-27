using System;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    public class TimelineControlTheme
    {
        public int TimelineWidth { get; set; }
        public int UnitMarkOffset { get; set; }
        public int UnitMarkLength { get; set; }
        public int UnitTextOffset { get; set; }
        public int SubUnitMarkOffset { get; set; }
        public int SubUnitMarkLength { get; set; }
        public int SubUnitTextOffset { get; set; }

        public SKColor TimelineColor { get; set; }
        public SKColor SummaryTextColor { get; set; }
        public SKColor UnitMarkColor { get; set; }

        public SKPaint TimelinePaint { get; set; }
        public SKPaint SummaryTextPaint { get; set; }
        public SKPaint UnitMarkPaint { get; set; }
        public SKPaint UnitTextPaint { get; set; }
        public SKPaint SubUnitTextPaint { get; set; }

        public TimelineControlTheme()
        {
            TimelineWidth = 130;
            UnitMarkOffset = 5;
            UnitMarkLength = 40;
            UnitTextOffset = 55;
            SubUnitMarkOffset = 5;
            SubUnitMarkLength = 15;
            SubUnitTextOffset = 10;

            TimelineColor = Color.SkyBlue.ToSKColor();
            SummaryTextColor = Color.Blue.ToSKColor();
            UnitMarkColor = Color.Black.ToSKColor();

            TimelinePaint = new SKPaint();
            TimelinePaint.Color = TimelineColor;
            TimelinePaint.StrokeWidth = TimelineWidth;

            SummaryTextPaint = new SKPaint();
            SummaryTextPaint.Color = SummaryTextColor;
            SummaryTextPaint.TextSize = 24;

            UnitMarkPaint = new SKPaint();
            UnitMarkPaint.Color = UnitMarkColor;
            UnitMarkPaint.StrokeWidth = 2;

            UnitTextPaint = new SKPaint();
            UnitTextPaint.Color = Color.Black.ToSKColor();
            UnitTextPaint.TextSize = 24;

            SubUnitTextPaint = new SKPaint();
            SubUnitTextPaint.Color = Color.Black.ToSKColor();
            SubUnitTextPaint.TextSize = 18;}
    }
}
