using System;
using Xamarin.Forms;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    public class TimelineControlTheme
    {
        int unitMarkOffset;
        int unitMarkLength;
        Point unitTextOffset;
        public int UnitMarkOffset { get { return unitMarkOffset; } }
        public int UnitMarkLength { get { return unitMarkLength; } }
        public Point UnitTextOffset { get { return unitTextOffset; } }

        int subUnitMarkOffset;
        int subUnitMarkLength;
        Point subUnitTextOffset;
        public int SubUnitMarkOffset { get { return subUnitMarkOffset; } }
        public int SubUnitMarkLength { get { return subUnitMarkLength; } }
        public Point SubUnitTextOffset { get { return subUnitTextOffset; } }

        SKPaint timelinePaint;
        SKPaint unitMarkPaint;
        SKPaint unitTextPaint;
        SKPaint subUnitMarkPaint;
        SKPaint subUnitTextPaint;
        float unitTextHalfHeight;
        float subUnitTextHalfHeight;

        public SKPaint TimelinePaint { 
            get { return timelinePaint; }
            set { timelinePaint = value; }
        }
        public SKPaint UnitMarkPaint { 
            get { return unitMarkPaint; }
            set { unitMarkPaint = value; }
        }
        public SKPaint UnitTextPaint { 
            get { return unitTextPaint; }
            set { unitTextPaint = value; unitTextHalfHeight = unitTextPaint.FontMetrics.CapHeight / 2; }
        }
        public SKPaint SubUnitMarkPaint { 
            get { return subUnitMarkPaint; }
            set { subUnitMarkPaint = value; }
        }
        public SKPaint SubUnitTextPaint { 
            get { return subUnitTextPaint; }
            set { subUnitTextPaint = value; subUnitTextHalfHeight = subUnitTextPaint.FontMetrics.CapHeight / 2; }
        }

        public float UnitTextHalfHeight {
            get { return unitTextHalfHeight; }
        }
        public float SubUnitTextHalfHeight {
            get { return subUnitTextHalfHeight; }
        }

        public TimelineControlTheme(TimelineOrientation orientation)
        {
            UnitMarkPaint = new SKPaint();
            UnitMarkPaint.Color = Color.Black.ToSKColor();
            UnitMarkPaint.StrokeWidth = 2;

            UnitTextPaint = new SKPaint();
            UnitTextPaint.Color = Color.Black.ToSKColor();
            UnitTextPaint.TextSize = 24;

            SubUnitMarkPaint = new SKPaint();
            SubUnitMarkPaint.Color = Color.Black.ToSKColor();
            SubUnitMarkPaint.StrokeWidth = 2;

            SubUnitTextPaint = new SKPaint();
            SubUnitTextPaint.Color = Color.Black.ToSKColor();
            SubUnitTextPaint.TextSize = 18;

            if (orientation == TimelineOrientation.Portrait)
            {
                TimelinePaint = new SKPaint();
                TimelinePaint.Color = Color.SkyBlue.ToSKColor();
                TimelinePaint.StrokeWidth = 130;

                unitMarkOffset = 5;
                unitMarkLength = 40;
                unitTextOffset = new Point(55, unitTextHalfHeight);

                subUnitMarkOffset = 5;
                subUnitMarkLength = 15;
                subUnitTextOffset = new Point(25, subUnitTextHalfHeight);
            }

            if(orientation==TimelineOrientation.Landscape)
            {
                TimelinePaint = new SKPaint();
                TimelinePaint.Color = Color.SkyBlue.ToSKColor();
                TimelinePaint.StrokeWidth = 80;

                unitMarkOffset = 0;
                unitMarkLength = 40;
                unitTextOffset = new Point(0, -45);

                subUnitMarkOffset = 0;
                subUnitMarkLength = 10;
                subUnitTextOffset = new Point(0, -15);
            }
        }
    }
}
