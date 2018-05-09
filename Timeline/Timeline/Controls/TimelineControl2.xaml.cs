using System;
using Xamarin.Forms;

using TouchTracking;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
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
        KYear,
        KKYear,
        KKKYear,
        MYear
    }

    public enum TimelineOrientation
    {
        Portrait,
        Landscape
    }

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

        TouchGestureRecognizer gestureRecognizer;
        TimelineOrientation orientation;
        TimelineControlTheme theme_portrait;
        TimelineControlTheme theme_landscape;

        TimelineControlDate date;
        TimelineControlDate unitDate;
        TimelineControlDate subUnitDate;

        SKPaint timelinePaint;
        SKPaint unitMarkPaint;
        SKPaint unitTextPaint;
        float unitTextHalfHeight;
        SKPaint subUnitMarkPaint;
        SKPaint subUnitTextPaint;
        float subUnitTextHalfHeight;

        Int64 pixeltime;
        float unitXWidth;
        float subunitXWidth;
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

        //FOR PORTRAIT MODE
        //1 hour : 3600 sec
        //1 day  : 86400 sec
        //1 month: 2592000 sec  (30 days)
        //1 year : 31104000 sec
        const int DEFAULT_PORTRAIT_HOUR_TO_DAY = 36;
        const int DEFAULT_PORTRAIT_DAY_TO_MONTH = 1728;
        const int DEFAULT_PORTRAIT_MONTH_TO_YEAR = 25920;
        const int DEFAULT_PORTRAIT_YEAR_TO_DECADE = 155520;

        const int DEFAULT_PORTRAIT_HOUR_SUBUNIT_LIMIT = 4;
        const int DEFAULT_PORTRAIT_HOUR_HALFSUBUNIT_LIMIT = 18;
        const int DEFAULT_PORTRAIT_DAY_SUBUNIT_LIMIT = 120;
        const int DEFAULT_PORTRAIT_DAY_HALFSUBUNIT_LIMIT = 576;
        const int DEFAULT_PORTRAIT_MONTH_SUBUNIT_LIMIT = 4320;
        const int DEFAULT_PORTRAIT_MONTH_HALFSUBUNIT_LIMIT = 9600;
        const int DEFAULT_PORTRAIT_YEAR_SUBUNIT_LIMIT = 64800;
        const int DEFAULT_PORTRAIT_YEAR_HALFSUBUNIT_LIMIT = 124000;
        const int DEFAULT_PORTRAIT_DECADE_SUBUNIT_LIMIT = 1036800;
        const int DEFAULT_PORTRAIT_DECADE_HALFSUBUNIT_LIMIT = 2000000;

        float timelineWidth;
        float timelineLeftX;
        float timelineMiddleX;
        float unitMarkX1;
        float unitMarkX2;
        float subUnitMarkX1;
        float subUnitMarkX2;
        float unitTextX;
        float subUnitTextX;
        int halfHeight;

        //FOR LANDSCAPE MODE
        //1 hour : 3600 sec
        //1 day  : 86400 sec
        //1 month: 2592000 sec  (30 days)
        //1 year : 31104000 sec
        const int DEFAULT_LANDSCAPE_HOUR_TO_DAY = 36;
        const int DEFAULT_LANDSCAPE_DAY_TO_MONTH = 432;
        const int DEFAULT_LANDSCAPE_MONTH_TO_YEAR = 12960;
        const int DEFAULT_LANDSCAPE_YEAR_TO_DECADE = 155520;

        const int DEFAULT_LANDSCAPE_HOUR_SUBUNIT_LIMIT = 2;
        const int DEFAULT_LANDSCAPE_HOUR_HALFSUBUNIT_LIMIT = 24;
        const int DEFAULT_LANDSCAPE_DAY_SUBUNIT_LIMIT = 120;
        const int DEFAULT_LANDSCAPE_DAY_HALFSUBUNIT_LIMIT = 300;
        const int DEFAULT_LANDSCAPE_MONTH_SUBUNIT_LIMIT = 2880;
        const int DEFAULT_LANDSCAPE_MONTH_HALFSUBUNIT_LIMIT = 9600;
        const int DEFAULT_LANDSCAPE_YEAR_SUBUNIT_LIMIT = 64800;
        const int DEFAULT_LANDSCAPE_YEAR_HALFSUBUNIT_LIMIT = 124000;
        const int DEFAULT_LANDSCAPE_DECADE_SUBUNIT_LIMIT = 1036800;
        const int DEFAULT_LANDSCAPE_DECADE_HALFSUBUNIT_LIMIT = 2000000;

        float timelineHeight;
        float timelineBottomY;
        float timelineMiddleY;
        float unitMarkY1;
        float unitMarkY2;
        float subUnitMarkY1;
        float subUnitMarkY2;
        float unitTextY;
        float subUnitTextY;
        int halfWidth;

        bool initialOrientationCheck;

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
            DateStr = date.DateStr(ZoomUnit);

            pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
            showSubUnitText = false;

            initialOrientationCheck = true;

            timelinePaint = new SKPaint();
            unitMarkPaint = new SKPaint();
            unitTextPaint = new SKPaint();
            subUnitMarkPaint = new SKPaint();
            subUnitTextPaint = new SKPaint();
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
                    date.baseDate = date.baseDate.AddTicks(-2 * (long)move * pixeltime);
                    DateStr = date.DateStr(ZoomUnit-1);
                    canvasView.InvalidateSurface();
                    break;

                case TouchGestureType.Pinch:
                    Console.WriteLine("PINCH - " + args.Data.ToString());
                    move = (orientation == TimelineOrientation.Portrait) ? args.Data.Y : args.Data.X;
                    Zoom -= Zoom * 0.005 * move;
                    if (Zoom < 1.5) Zoom = 1.5;
                    pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
                    Console.WriteLine("Zoom: " + Zoom.ToString() + "  pixeltime: " + pixeltime.ToString());
                    AdjustZoomUnit();
                    DateStr = date.DateStr(ZoomUnit - 1);
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
            CheckOrientation(info, initialOrientationCheck);

            canvas.Clear();

            if (orientation == TimelineOrientation.Portrait)
            {
                //BASELINE
                canvas.DrawLine(timelineMiddleX, 0, timelineMiddleX, info.Height, timelinePaint);
                //UNITS AND SUBUNITS
                DrawUnitsAndSubUnitsPortrait(info, canvas);
                //HIGHLIGHTER
                canvas.DrawLine(timelineLeftX, halfHeight, info.Width, halfHeight, theme_portrait.HighlightPaint);
            }
            else
            {
                //BASELINE
                canvas.DrawLine(0, timelineMiddleY, info.Width, timelineMiddleY, timelinePaint);
                //UNITS AND SUBUNITS
                DrawUnitsAndSubUnitsLandscape(info, canvas);
                //HIGHLIGHTER
                canvas.DrawLine(halfWidth, 0, halfWidth, timelineBottomY, theme_landscape.HighlightPaint);
            }
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

                //UNIT MARK
                canvas.DrawLine(unitMarkX1, unitPos, unitMarkX2, unitPos, unitMarkPaint);
                //UNIT TEXT
                canvas.DrawText(GetUnitText(unitDate), unitTextX, unitPos + unitTextHalfHeight, unitTextPaint);

                unitDate.CopyByUnit(ref subUnitDate, ZoomUnit - 1);

                while(subUnitDate.Value(ZoomUnit) == unitDate.Value(ZoomUnit) && subUnitDate.baseDate.Ticks < maxDate.Ticks)
                {
                    subUnitPos = (subUnitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                    //SUBUNIT MARK
                    canvas.DrawLine(subUnitMarkX1, subUnitPos, subUnitMarkX2, subUnitPos, subUnitMarkPaint);

                    if (showSubUnitText)
                    {
                        //SUBUNIT TEXT
                        //canvas.DrawText(GetSubUnitText(subUnitDate), subUnitTextX, subUnitPos + subUnitTextHalfHeight, subUnitTextPaint);
                    }
                    else if (showMiddleSubUnitText && IsMiddleSubUnit(subUnitDate))
                    {
                        //ONLY MIDDLE SUBUNIT TEXT
                        canvas.DrawText(GetSubUnitText(subUnitDate), subUnitTextX, subUnitPos + subUnitTextHalfHeight, subUnitTextPaint);
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
            string unitText;
            string subUnitText;
            float unitTextWidthHalf;
            float subUnitTextWidthHalf;

            DateTime minDate = new DateTime(date.baseDate.Ticks - halfWidth * pixeltime);
            DateTime maxDate = new DateTime(date.baseDate.Ticks + halfWidth * pixeltime);

            date.CopyByUnit(ref unitDate, ZoomUnit);

            while (unitDate.baseDate.Ticks > minDate.Ticks)
                unitDate.Add(ZoomUnit, -1);

            while (unitDate.baseDate.Ticks < maxDate.Ticks)
            {
                unitPos = (unitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                unitText = GetUnitText(unitDate);
                unitTextWidthHalf = unitText.Length * unitXWidth / 2;

                //UNIT MARK
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, theme_landscape.UnitMarkPaint);
                //UNIT TEXT
                canvas.DrawText(unitText, unitPos - unitTextWidthHalf, unitTextY, theme_landscape.UnitTextPaint);

                unitDate.CopyByUnit(ref subUnitDate, ZoomUnit - 1);

                while (subUnitDate.Value(ZoomUnit) == unitDate.Value(ZoomUnit) && subUnitDate.baseDate.Ticks < maxDate.Ticks)
                {
                    subUnitPos = (subUnitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                    subUnitText = GetSubUnitText(subUnitDate);
                    subUnitTextWidthHalf = subUnitText.Length * subunitXWidth / 2;

                    //SUBUNIT MARK
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, theme_landscape.UnitMarkPaint);

                    if (showSubUnitText)
                    {
                        //SUBUNIT TEXT
                        canvas.DrawText(subUnitText, subUnitPos - subUnitTextWidthHalf, subUnitTextY, theme_landscape.UnitTextPaint);
                    }
                    else if (showMiddleSubUnitText && IsMiddleSubUnit(subUnitDate))
                    {
                        //ONLY MIDDLE SUBUNIT TEXT
                        canvas.DrawText(subUnitText, subUnitPos - subUnitTextWidthHalf, subUnitTextY, theme_landscape.UnitTextPaint);
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
                    if (orientation == TimelineOrientation.Portrait)
                    {
                        //PORTRAIT
                        if (tlcdate.baseDate.Minute == 0) return "";
                        if(tlcdate.baseDate.Minute % 5 == 0) return tlcdate.baseDate.Hour.ToString("00") + ":" + tlcdate.baseDate.Minute.ToString("00");
                        return "";
                    } else 
                    {
                        //LANDSCAPE
                        if (tlcdate.baseDate.Minute == 0) return "";
                        return tlcdate.baseDate.Minute.ToString();
                    }
                case TimelineUnits.Day:
                    if (orientation == TimelineOrientation.Portrait)
                    {
                        //PORTRAIT
                        if (tlcdate.baseDate.Hour == 0) return "";
                        return tlcdate.baseDate.Hour.ToString() + ":00";
                    } else 
                    {
                        //LANDSCAPE
                        if (tlcdate.baseDate.Hour == 0) return "";
                        return tlcdate.baseDate.Hour.ToString();
                    }
                case TimelineUnits.Month:
                    if (tlcdate.baseDate.Day == 1) return "";
                    return tlcdate.baseDate.Day.ToString();
                case TimelineUnits.Year:
                    if (tlcdate.baseDate.Month == 1) return "";
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
        private void CheckOrientation(SKImageInfo info, bool forceSet = false)
        {
            TimelineOrientation oldorientation = orientation;

            orientation = TimelineOrientation.Portrait;
            if (this.Bounds.Width > this.Bounds.Height) orientation = TimelineOrientation.Landscape;

            if ((oldorientation != orientation) || forceSet)
            {
                if (orientation == TimelineOrientation.Portrait)
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

                    timelineWidth = (float)(info.Width * 0.15);
                    timelineLeftX = info.Width - timelineWidth;
                    timelineMiddleX = timelineLeftX + timelineWidth / 2;
                    unitMarkX1 = timelineLeftX; // + theme_portrait.UnitMarkOffset;
                    unitMarkX2 = unitMarkX1 + 30; // + theme_portrait.UnitMarkLength;
                    subUnitMarkX1 = timelineLeftX; // + theme_portrait.SubUnitMarkOffset;
                    subUnitMarkX2 = subUnitMarkX1 + 15; // + theme_portrait.SubUnitMarkLength;
                    unitTextX = timelineLeftX + 35; // + (float)theme_portrait.UnitTextOffset.X;
                    subUnitTextX = timelineLeftX + 20; // + (float)theme_portrait.SubUnitTextOffset.X;
                    halfHeight = info.Height / 2;

                    unitXWidth = theme_portrait.UnitTextPaint.MeasureText("X");
                    subunitXWidth = theme_portrait.SubUnitTextPaint.MeasureText("X");

                    timelinePaint.Color = Color.SkyBlue.ToSKColor();
                    timelinePaint.StrokeWidth = timelineWidth;
                    unitMarkPaint.Color = Color.Black.ToSKColor();
                    unitMarkPaint.StrokeWidth = 2;
                    unitTextPaint.Color = Color.Black.ToSKColor();
                    unitTextPaint.TextSize = GetTextSizeForWidth(timelineWidth-40);
                    unitTextHalfHeight = unitTextPaint.FontMetrics.CapHeight / 2;
                    subUnitMarkPaint.Color = Color.Black.ToSKColor();
                    subUnitMarkPaint.StrokeWidth = 2;
                    subUnitTextPaint.Color = Color.Black.ToSKColor();
                    subUnitTextPaint.TextSize = unitTextPaint.TextSize - 4;
                    subUnitTextHalfHeight = subUnitTextPaint.FontMetrics.CapHeight / 2;
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

                    timelineHeight = (float)(info.Height * 0.15);
                    timelineBottomY = timelineHeight;
                    timelineMiddleY = timelineBottomY - timelineHeight / 2;
                    unitMarkY1 = timelineBottomY - theme_landscape.UnitMarkOffset;
                    unitMarkY2 = unitMarkY1 - theme_landscape.UnitMarkLength;
                    subUnitMarkY1 = timelineBottomY - theme_landscape.SubUnitMarkOffset;
                    subUnitMarkY2 = subUnitMarkY1 - theme_landscape.SubUnitMarkLength;
                    unitTextY = timelineBottomY - (float)theme_landscape.UnitTextOffset.Y;
                    subUnitTextY = timelineBottomY - (float)theme_landscape.SubUnitTextOffset.Y;
                    halfWidth = info.Width / 2;

                    unitXWidth = theme_landscape.UnitTextPaint.MeasureText("X");
                    subunitXWidth = theme_landscape.SubUnitTextPaint.MeasureText("X");
                }

                initialOrientationCheck = false;
            }
        }

        //Any touch action we simply forward to the gesture recognizer
        protected void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, args.Location.ToSKPoint());
        }

        private int GetTextSizeForWidth(float width)
        {
            if (width < 40) return 20;
            if (width < 60) return 24;
            if (width < 80) return 28;
            if (width < 100) return 32;
            if (width < 120) return 36;
            if (width < 140) return 40;
            if (width < 160) return 44;
            return 44;
        }
    }
}
