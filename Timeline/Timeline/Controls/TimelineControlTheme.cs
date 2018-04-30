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

        public SKColor UnitMarkColor { get; set; }
        public SKColor UnitTextColor { get; set; }

        public SKColor SubUnitMarkColor { get; set; }
        public SKColor SubUnitTextColor { get; set; }

        public SKPaint TimelinePaint { get; set; }
        public SKPaint SummaryTextPaint { get; set; }
        public SKPaint UnitMarkPaint { get; set; }
        public SKPaint UnitTextPaint { get; set; }
        public SKPaint SubUnitMarkPaint { get; set; }
        public SKPaint SubUnitTextPaint { get; set; }

        public TimelineControlTheme(TimelineOrientation orientation)
        {
            if (orientation == TimelineOrientation.Portrait)
            {
                TimelineWidth = 130;

                UnitMarkOffset = 5;
                UnitMarkLength = 40;
                UnitTextOffset = 55;

                SubUnitMarkOffset = 5;
                SubUnitMarkLength = 15;
                SubUnitTextOffset = 10;

                TimelineColor = Color.SkyBlue.ToSKColor();

                UnitMarkColor = Color.Black.ToSKColor();
                UnitTextColor = Color.Black.ToSKColor();

                SubUnitMarkColor = Color.Black.ToSKColor();
                SubUnitTextColor = Color.Black.ToSKColor();
            }

            if(orientation==TimelineOrientation.Landscape)
            {
                TimelineWidth = 80;

                UnitMarkOffset = 0;
                UnitMarkLength = 40;
                UnitTextOffset = 45;

                SubUnitMarkOffset = 0;
                SubUnitMarkLength = 10;
                SubUnitTextOffset = 15;

                TimelineColor = Color.SkyBlue.ToSKColor();

                UnitMarkColor = Color.Black.ToSKColor();
                UnitTextColor = Color.Black.ToSKColor();

                SubUnitMarkColor = Color.Black.ToSKColor();
                SubUnitTextColor = Color.Black.ToSKColor();
            }

            ApplyChanges();
        }

        public void ApplyChanges()
        {
            TimelinePaint = new SKPaint();
            TimelinePaint.Color = TimelineColor;
            TimelinePaint.StrokeWidth = TimelineWidth;

            UnitMarkPaint = new SKPaint();
            UnitMarkPaint.Color = UnitMarkColor;
            UnitMarkPaint.StrokeWidth = 2;

            UnitTextPaint = new SKPaint();
            UnitTextPaint.Color = UnitTextColor;
            UnitTextPaint.TextSize = 24;

            SubUnitMarkPaint = new SKPaint();
            SubUnitMarkPaint.Color = SubUnitMarkColor;
            SubUnitMarkPaint.StrokeWidth = 2;

            SubUnitTextPaint = new SKPaint();
            SubUnitTextPaint.Color = SubUnitTextColor;
            SubUnitTextPaint.TextSize = 18;
        }
    }
}
