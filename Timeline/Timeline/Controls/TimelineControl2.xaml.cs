﻿using System;
using Xamarin.Forms;

using TouchTracking;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    //public enum TimelineUnits
    //{
    //    Minute,
    //    Hour,
    //    Day,
    //    Month,
    //    Year,
    //    Decade,
    //    Century,
    //    KYear,
    //    KKYear,
    //    KKKYear,
    //    MYear
    //}

    //public enum TimelineOrientation
    //{
    //    Portrait,
    //    Landscape
    //}

    public partial class TimelineControl2 : ContentView
    {

        #region "Bindable properties"
        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(double),
            typeof(TimelineControl2),
            (double)10, BindingMode.OneWay,
            propertyChanged: OnZoomChanged);

        public static readonly BindableProperty ZoomUnitProperty = BindableProperty.Create(
            nameof(ZoomUnit),
            typeof(TimelineUnits),
            typeof(TimelineControl2),
            TimelineUnits.Hour, BindingMode.OneWay,
            propertyChanged: OnZoomUnitChanged);

        public static readonly BindableProperty DateStrProperty = BindableProperty.Create(
            nameof(DateStr),
            typeof(string),
            typeof(TimelineControl2),
            "", BindingMode.OneWay);

        private static void OnZoomChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl2)bindable).InvalidateLayout();
        }

        private static void OnZoomUnitChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl2)bindable).InvalidateLayout();
        }

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public TimelineUnits ZoomUnit
        {
            get { return (TimelineUnits)GetValue(ZoomUnitProperty); }
            set { SetValue(ZoomUnitProperty, value); }
        }

        public string DateStr
        {
            get { return (string)GetValue(DateStrProperty); }
            set { SetValue(DateStrProperty, value); }
        }

        #endregion

        string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        const int DEFAULT_PORTRAIT_HOUR_TO_DAY = 36;
        const int DEFAULT_PORTRAIT_DAY_TO_MONTH = 432;
        const int DEFAULT_PORTRAIT_MONTH_TO_YEAR = 12960;
        const int DEFAULT_PORTRAIT_YEAR_TO_DECADE = 155520;
        const int DEFAULT_LANDSCAPE_HOUR_TO_DAY = 36;
        const int DEFAULT_LANDSCAPE_DAY_TO_MONTH = 432;
        const int DEFAULT_LANDSCAPE_MONTH_TO_YEAR = 12960;
        const int DEFAULT_LANDSCAPE_YEAR_TO_DECADE = 155520;

        const int DEFAULT_PORTRAIT_HOUR_SUBUNIT_LIMIT = 5;
        const int DEFAULT_PORTRAIT_HOUR_HALFSUBUNIT_LIMIT = 24;
        const int DEFAULT_PORTRAIT_DAY_SUBUNIT_LIMIT = 120;
        const int DEFAULT_PORTRAIT_DAY_HALFSUBUNIT_LIMIT = 300;
        const int DEFAULT_PORTRAIT_MONTH_SUBUNIT_LIMIT = 2880;
        const int DEFAULT_PORTRAIT_MONTH_HALFSUBUNIT_LIMIT = 9600;
        const int DEFAULT_PORTRAIT_YEAR_SUBUNIT_LIMIT = 64800;
        const int DEFAULT_PORTRAIT_YEAR_HALFSUBUNIT_LIMIT = 124000;
        const int DEFAULT_PORTRAIT_DECADE_SUBUNIT_LIMIT = 1036800;
        const int DEFAULT_PORTRAIT_DECADE_HALFSUBUNIT_LIMIT = 2000000;

        const int DEFAULT_LANDSCAPE_HOUR_SUBUNIT_LIMIT = 5;
        const int DEFAULT_LANDSCAPE_HOUR_HALFSUBUNIT_LIMIT = 24;
        const int DEFAULT_LANDSCAPE_DAY_SUBUNIT_LIMIT = 120;
        const int DEFAULT_LANDSCAPE_DAY_HALFSUBUNIT_LIMIT = 300;
        const int DEFAULT_LANDSCAPE_MONTH_SUBUNIT_LIMIT = 2880;
        const int DEFAULT_LANDSCAPE_MONTH_HALFSUBUNIT_LIMIT = 9600;
        const int DEFAULT_LANDSCAPE_YEAR_SUBUNIT_LIMIT = 64800;
        const int DEFAULT_LANDSCAPE_YEAR_HALFSUBUNIT_LIMIT = 124000;
        const int DEFAULT_LANDSCAPE_DECADE_SUBUNIT_LIMIT = 1036800;
        const int DEFAULT_LANDSCAPE_DECADE_HALFSUBUNIT_LIMIT = 2000000;

        TouchGestureRecognizer gestureRecognizer;
        TimelineOrientation orientation;
        TimelineControlTheme theme_portrait;
        TimelineControlTheme theme_landscape;

        TimelineControlDate date;
        TimelineControlDate unitDate;
        TimelineControlDate subUnitDate;

        Int64 pixeltime;
        bool showSubUnitText;
        bool showMiddleSubUnitText;

        int hourToDayZoomLimit;
        int dayToMonthZoomLimit;
        int monthToYearZoomLimit;
        int yearToDecadeZoomLimit;

        int hourSubunitLimit;
        int hourHalfSubUnitLimit;
        int daySubunitLimit;
        int dayHalfSubUnitLimit;
        int monthSubunitLimit;
        int monthHalfSubUnitLimit;
        int yearSubunitLimit;
        int yearHalfSubUnitLimit;
        int decadeSubunitLimit;
        int decadeHalfSubUnitLimit;

        //for portrait mode
        int timelineLeftPos;
        int timelineMiddleX;
        int unitMarkX1;
        int unitMarkX2;
        int subUnitMarkX1;
        int subUnitMarkX2;
        int unitTextX;
        int subUnitTextX;
        int halfHeight;

        //for landscape mode
        int timelineBottomPos;
        int timelineMiddleY;
        int unitMarkY1;
        int unitMarkY2;
        int subUnitMarkY1;
        int subUnitMarkY2;
        int unitTextY;
        int subUnitTextY;
        int halfWidth;

        public TimelineControl2()
        {
            InitializeComponent();
            gestureRecognizer = new TouchGestureRecognizer();
            gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

            theme_portrait = new TimelineControlTheme(TimelineOrientation.Portrait);
            theme_landscape = new TimelineControlTheme(TimelineOrientation.Landscape);

            date = new TimelineControlDate();
            unitDate = new TimelineControlDate();
            subUnitDate = new TimelineControlDate();
            DateStr = date.DateStr();

            pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
            showSubUnitText = false;

            CheckOrientation();
        }

        void GestureRecognizer_OnGestureRecognized(object sender, TouchGestureEventArgs args)
        {
            float move;

            switch (args.Type)
            {
                case TouchGestureType.Tap:
                    Console.WriteLine("TAP");
                    break;

                case TouchGestureType.LongTap:
                    Console.WriteLine("LONGTAP");
                    break;

                case TouchGestureType.Pan:
                    Console.WriteLine("PAN");
                    move = (orientation == TimelineOrientation.Portrait) ? args.Data.Y : args.Data.X;
                    date.baseDate = date.baseDate.AddTicks(-1 * (long)move * pixeltime);
                    DateStr = date.DateStr();
                    canvasView.InvalidateSurface();
                    break;

                case TouchGestureType.Pinch:
                    Console.WriteLine("PINCH - " + args.Data.ToString());
                    move = (orientation == TimelineOrientation.Portrait) ? args.Data.Y : args.Data.X;
                    Zoom -= Zoom * 0.005 * move;
                    if (Zoom < 2) Zoom = 2;
                    pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
                    Console.WriteLine("Zoom: " + Zoom.ToString() + "  pixeltime: " + pixeltime.ToString());
                    AdjustZoomUnit();
                    canvasView.InvalidateSurface();
                    break;

                case TouchGestureType.Swipe:
                    Console.WriteLine("SWIPE");
                    break;
            }

        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            //the user might have rotated the phone
            CheckOrientation();

            canvas.Clear();

            //DRAW TIMELINE BACKGROUND

            if (orientation == TimelineOrientation.Portrait)
            {
                timelineLeftPos = info.Width - theme_portrait.TimelineWidth;
                timelineMiddleX = timelineLeftPos + theme_portrait.TimelineWidth / 2;
                canvas.DrawLine(timelineMiddleX, 0, timelineMiddleX, info.Height, theme_portrait.TimelinePaint);

                //DRAW UNITS
                unitMarkX1 = timelineLeftPos + theme_portrait.UnitMarkOffset;
                unitMarkX2 = unitMarkX1 + theme_portrait.UnitMarkLength;
                subUnitMarkX1 = timelineLeftPos + theme_portrait.SubUnitMarkOffset;
                subUnitMarkX2 = subUnitMarkX1 + theme_portrait.SubUnitMarkLength;
                unitTextX = timelineLeftPos + theme_portrait.UnitTextOffset;
                subUnitTextX = timelineLeftPos + theme_portrait.SubUnitTextOffset;
                halfHeight = info.Height / 2;

                DrawUnitsAndSubUnitsPortrait(info, canvas);
            }
            else
            {
                timelineBottomPos = theme_landscape.TimelineWidth;
                timelineMiddleY = timelineBottomPos - theme_landscape.TimelineWidth / 2;
                canvas.DrawLine(0, timelineMiddleY, info.Width, timelineMiddleY, theme_landscape.TimelinePaint);

                //DRAW UNITS
                unitMarkY1 = timelineBottomPos - theme_landscape.UnitMarkOffset;
                unitMarkY2 = unitMarkY1 - theme_landscape.UnitMarkLength;
                subUnitMarkY1 = timelineBottomPos - theme_landscape.SubUnitMarkOffset;
                subUnitMarkY2 = subUnitMarkY1 - theme_landscape.SubUnitMarkLength;
                unitTextY = timelineBottomPos - theme_landscape.UnitTextOffset;
                subUnitTextY = timelineBottomPos - theme_landscape.SubUnitTextOffset;
                halfWidth = info.Width / 2;

                DrawUnitsAndSubUnitsLandscape(info, canvas);
            }

            //canvas.DrawLine(0, info.Height / 2, info.Width, info.Height / 2, theme.UnitMarkPaint);

        }

        private void DrawUnitsAndSubUnitsPortrait(SKImageInfo info, SKCanvas canvas)
        {
            float unitPos;
            float subUnitPos;

            DateTime minDate = new DateTime(date.baseDate.Ticks - halfHeight * pixeltime);
            DateTime maxDate = new DateTime(date.baseDate.Ticks + halfHeight * pixeltime);

            date.CopyByUnit(ref unitDate, ZoomUnit);

            while (unitDate.baseDate.Ticks > minDate.Ticks)
                unitDate.Add(ZoomUnit, -1);

            while (unitDate.baseDate.Ticks < maxDate.Ticks)
            {
                unitPos = (unitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                canvas.DrawLine(unitMarkX1, unitPos, unitMarkX2, unitPos, theme_portrait.UnitMarkPaint);
                canvas.DrawText(GetUnitText(unitDate), unitTextX, unitPos + theme_portrait.UnitTextPaint.FontMetrics.CapHeight / 2, theme_portrait.UnitTextPaint);

                unitDate.CopyByUnit(ref subUnitDate, ZoomUnit - 1);

                while(subUnitDate.Value(ZoomUnit) == unitDate.Value(ZoomUnit) && subUnitDate.baseDate.Ticks < maxDate.Ticks)
                {
                    subUnitPos = (subUnitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                    canvas.DrawLine(subUnitMarkX1, subUnitPos, subUnitMarkX2, subUnitPos, theme_portrait.SubUnitMarkPaint);

                    if (showSubUnitText)
                    {
                        canvas.DrawText(GetSubUnitText(subUnitDate), subUnitTextX, subUnitPos + theme_portrait.SubUnitTextPaint.TextSize, theme_portrait.SubUnitTextPaint);
                    }
                    else if (showMiddleSubUnitText && IsMiddleSubUnit(subUnitDate))
                    {
                        canvas.DrawText(GetSubUnitText(subUnitDate), subUnitTextX, subUnitPos + theme_portrait.SubUnitTextPaint.TextSize, theme_portrait.SubUnitTextPaint);
                    }

                    subUnitDate.Add(ZoomUnit - 1);
                }

                unitDate.Add(ZoomUnit);
            }
        }

        private void DrawUnitsAndSubUnitsLandscape(SKImageInfo info, SKCanvas canvas)
        {
            float unitPos;
            float subUnitPos;

            DateTime minDate = new DateTime(date.baseDate.Ticks - halfWidth * pixeltime);
            DateTime maxDate = new DateTime(date.baseDate.Ticks + halfWidth * pixeltime);

            date.CopyByUnit(ref unitDate, ZoomUnit);

            while (unitDate.baseDate.Ticks > minDate.Ticks)
                unitDate.Add(ZoomUnit, -1);

            while (unitDate.baseDate.Ticks < maxDate.Ticks)
            {
                unitPos = (unitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, theme_landscape.UnitMarkPaint);
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, theme_landscape.UnitTextPaint);

                unitDate.CopyByUnit(ref subUnitDate, ZoomUnit - 1);

                while (subUnitDate.Value(ZoomUnit) == unitDate.Value(ZoomUnit) && subUnitDate.baseDate.Ticks < maxDate.Ticks)
                {
                    subUnitPos = (subUnitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, theme_landscape.UnitMarkPaint);

                    if (showSubUnitText)
                    {
                        canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + theme_landscape.UnitTextPaint.TextSize, subUnitTextY, theme_landscape.UnitTextPaint);
                    }
                    else if (showMiddleSubUnitText && IsMiddleSubUnit(subUnitDate))
                    {
                        canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + theme_landscape.UnitTextPaint.TextSize, subUnitTextY, theme_landscape.UnitTextPaint);
                    }
                    subUnitDate.Add(ZoomUnit - 1);
                }

                unitDate.Add(ZoomUnit);
            }
        }

        private string GetUnitText(TimelineControlDate tlcdate)
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return "";
                case TimelineUnits.Hour:
                    return tlcdate.baseDate.Hour.ToString() + ":00";
                case TimelineUnits.Day:
                    return tlcdate.baseDate.Day.ToString() + "." + shortMonthNames[tlcdate.baseDate.Month - 1];
                case TimelineUnits.Month:
                    return shortMonthNames[tlcdate.baseDate.Month - 1];
                case TimelineUnits.Year:
                    return tlcdate.baseDate.Year.ToString();
                case TimelineUnits.Decade:
                    return tlcdate.Decade.ToString() + "0";
                case TimelineUnits.Century:
                    return tlcdate.Century.ToString() + "00";
                default:
                    return "";
            }
        }

        private string GetSubUnitText(TimelineControlDate tlcdate)
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return "";
                case TimelineUnits.Hour:
                    return tlcdate.baseDate.Hour.ToString() + ":" + tlcdate.baseDate.Minute.ToString();
                case TimelineUnits.Day:
                    return tlcdate.baseDate.Hour.ToString() + ":00";
                case TimelineUnits.Month:
                    return tlcdate.baseDate.Day.ToString();
                case TimelineUnits.Year:
                    return shortMonthNames[tlcdate.baseDate.Month - 1];
                case TimelineUnits.Decade:
                    if (tlcdate.baseDate.Year % 10 == 0) return "";
                    return tlcdate.baseDate.Year.ToString();
                case TimelineUnits.Century:
                    return tlcdate.Decade.ToString() + "0";
                default:
                    return "";
            }
        }

        private bool IsMiddleSubUnit(TimelineControlDate tlcdate)
        {
            switch (this.ZoomUnit-1)
            {
                case TimelineUnits.Minute:
                    return tlcdate.baseDate.Minute % 10 == 0;
                case TimelineUnits.Hour:
                    return tlcdate.baseDate.Hour == 12;
                case TimelineUnits.Day:
                    return false;
                case TimelineUnits.Month:
                    return false;
                case TimelineUnits.Year:
                    return tlcdate.baseDate.Year % 5 == 0;
                case TimelineUnits.Decade:
                    return false;
                case TimelineUnits.Century:
                    return false;
                default:
                    return false;
            }
        }

        private void AdjustZoomUnit()
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    break;

                case TimelineUnits.Hour:
                    if (Zoom > hourToDayZoomLimit) ZoomUnit = TimelineUnits.Day;
                    showSubUnitText = (Zoom < hourSubunitLimit) ? true : false;
                    showMiddleSubUnitText = (Zoom < hourHalfSubUnitLimit) ? true : false;
                    break;

                case TimelineUnits.Day:
                    if (Zoom > dayToMonthZoomLimit) ZoomUnit = TimelineUnits.Month;
                    if (Zoom < hourToDayZoomLimit) ZoomUnit = TimelineUnits.Hour;
                    showSubUnitText = (Zoom < daySubunitLimit) ? true : false;
                    showMiddleSubUnitText = (Zoom < dayHalfSubUnitLimit) ? true : false;
                    break;

                case TimelineUnits.Month:
                    if (Zoom > monthToYearZoomLimit) ZoomUnit = TimelineUnits.Year;
                    if (Zoom < dayToMonthZoomLimit) ZoomUnit = TimelineUnits.Day;
                    showSubUnitText = (Zoom < monthSubunitLimit) ? true : false;
                    showMiddleSubUnitText = (Zoom < monthHalfSubUnitLimit) ? true : false;
                    break;

                case TimelineUnits.Year:
                    if (Zoom > yearToDecadeZoomLimit) ZoomUnit = TimelineUnits.Decade;
                    if (Zoom < monthToYearZoomLimit) ZoomUnit = TimelineUnits.Month;
                    showSubUnitText = (Zoom < yearSubunitLimit) ? true : false;
                    showMiddleSubUnitText = (Zoom < yearHalfSubUnitLimit) ? true : false;
                    break;

                case TimelineUnits.Decade:
                    if (Zoom < yearToDecadeZoomLimit) ZoomUnit = TimelineUnits.Year;
                    showSubUnitText = (Zoom < decadeSubunitLimit) ? true : false;
                    showMiddleSubUnitText = (Zoom < decadeHalfSubUnitLimit) ? true : false;
                    break;
            }
        }

        //Detect orientation
        private void CheckOrientation()
        {
            orientation = TimelineOrientation.Portrait;
            if (this.Bounds.Width > this.Bounds.Height) orientation = TimelineOrientation.Landscape;

            if(orientation==TimelineOrientation.Portrait)
            {
                hourToDayZoomLimit = DEFAULT_PORTRAIT_HOUR_TO_DAY;
                dayToMonthZoomLimit = DEFAULT_PORTRAIT_DAY_TO_MONTH;
                monthToYearZoomLimit = DEFAULT_PORTRAIT_MONTH_TO_YEAR;
                yearToDecadeZoomLimit = DEFAULT_PORTRAIT_YEAR_TO_DECADE;

                hourSubunitLimit = DEFAULT_PORTRAIT_HOUR_SUBUNIT_LIMIT;
                hourHalfSubUnitLimit = DEFAULT_PORTRAIT_HOUR_HALFSUBUNIT_LIMIT;
                daySubunitLimit = DEFAULT_PORTRAIT_DAY_SUBUNIT_LIMIT;
                dayHalfSubUnitLimit = DEFAULT_PORTRAIT_DAY_HALFSUBUNIT_LIMIT;
                monthSubunitLimit = DEFAULT_PORTRAIT_MONTH_SUBUNIT_LIMIT;
                monthHalfSubUnitLimit = DEFAULT_PORTRAIT_MONTH_HALFSUBUNIT_LIMIT;
                yearSubunitLimit = DEFAULT_PORTRAIT_YEAR_SUBUNIT_LIMIT;
                yearHalfSubUnitLimit = DEFAULT_PORTRAIT_YEAR_HALFSUBUNIT_LIMIT;
                decadeSubunitLimit = DEFAULT_PORTRAIT_DECADE_SUBUNIT_LIMIT;
                decadeHalfSubUnitLimit = DEFAULT_PORTRAIT_DECADE_HALFSUBUNIT_LIMIT;
            }
            else
            {
                hourToDayZoomLimit = DEFAULT_LANDSCAPE_HOUR_TO_DAY;
                dayToMonthZoomLimit = DEFAULT_LANDSCAPE_DAY_TO_MONTH;
                monthToYearZoomLimit = DEFAULT_LANDSCAPE_MONTH_TO_YEAR;
                yearToDecadeZoomLimit = DEFAULT_LANDSCAPE_YEAR_TO_DECADE;

                hourSubunitLimit = DEFAULT_LANDSCAPE_HOUR_SUBUNIT_LIMIT;
                hourHalfSubUnitLimit = DEFAULT_LANDSCAPE_HOUR_HALFSUBUNIT_LIMIT;
                daySubunitLimit = DEFAULT_LANDSCAPE_DAY_SUBUNIT_LIMIT;
                dayHalfSubUnitLimit = DEFAULT_LANDSCAPE_DAY_HALFSUBUNIT_LIMIT;
                monthSubunitLimit = DEFAULT_LANDSCAPE_MONTH_SUBUNIT_LIMIT;
                monthHalfSubUnitLimit = DEFAULT_LANDSCAPE_MONTH_HALFSUBUNIT_LIMIT;
                yearSubunitLimit = DEFAULT_LANDSCAPE_YEAR_SUBUNIT_LIMIT;
                yearHalfSubUnitLimit = DEFAULT_LANDSCAPE_YEAR_HALFSUBUNIT_LIMIT;
                decadeSubunitLimit = DEFAULT_LANDSCAPE_DECADE_SUBUNIT_LIMIT;
                decadeHalfSubUnitLimit = DEFAULT_LANDSCAPE_DECADE_HALFSUBUNIT_LIMIT;
            }
        }

        //Any touch action we simply forward to the gesture recognizer
        protected void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, args.Location.ToSKPoint());
        }
    }
}
