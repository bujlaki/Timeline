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

        int hourToDayZoomLimit;
        int dayToMonthZoomLimit;
        int monthToYearZoomLimit;
        int yearToDecadeZoomLimit;

        int hourSubunitLimit;
        int daySubunitLimit;
        int monthSubunitLimit;
        int yearSubunitLimit;
        int decadeSubunitLimit;

		const int SEC_PER_MINUTE = 60;
		const int SEC_PER_HOUR = 3600;
		const int SEC_PER_DAY = 86400;
		const int SEC_PER_MONTH = 2592000;
		const int SEC_PER_YEAR = 31104000;

        //FOR PORTRAIT MODE
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
                    if (Zoom < 4) Zoom = 4;
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

					//SUBUNIT TEXT
                    if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitTextX, subUnitPos + subUnitTextHalfHeight, subUnitTextPaint);

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
                //unitTextWidthHalf = unitText.Length * unitXWidth / 2;

                //UNIT MARK
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, unitMarkPaint);
                //UNIT TEXT
                //canvas.DrawText(unitText, unitPos - unitTextWidthHalf, unitTextY, unitTextPaint);
				canvas.DrawText(unitText, unitPos, unitTextY, unitTextPaint);

                unitDate.CopyByUnit(ref subUnitDate, ZoomUnit - 1);

                while (subUnitDate.Value(ZoomUnit) == unitDate.Value(ZoomUnit) && subUnitDate.baseDate.Ticks < maxDate.Ticks)
                {
                    subUnitPos = (subUnitDate.baseDate.Ticks - minDate.Ticks) / pixeltime;
                    subUnitText = GetSubUnitText(subUnitDate);
                    //subUnitTextWidthHalf = subUnitText.Length * subunitXWidth / 2;
                    
                    //SUBUNIT MARK
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, subUnitMarkPaint);

					//SUBUNIT TEXT
                    //if (showSubUnitText) canvas.DrawText(subUnitText, subUnitPos - subUnitTextWidthHalf, subUnitTextY, subUnitTextPaint);
					if (showSubUnitText) canvas.DrawText(subUnitText, subUnitPos + 5, subUnitTextY, subUnitTextPaint);

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
			if(orientation==TimelineOrientation.Portrait){
				//PORTRAIT
				switch (this.ZoomUnit)
                {
                    case TimelineUnits.Minute:
                        return "";
                    case TimelineUnits.Hour:
                        if (tlcdate.baseDate.Minute == 0) return "";
                        if (tlcdate.baseDate.Minute % 5 == 0) return tlcdate.baseDate.Hour.ToString("00") + ":" + tlcdate.baseDate.Minute.ToString("00");
                        return "";
                    case TimelineUnits.Day:
                        if (tlcdate.baseDate.Hour == 0) return "";
                        return tlcdate.baseDate.Hour.ToString() + ":00";
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
			} else {
				//LANDSCAPE
				switch (this.ZoomUnit)
                {
                    case TimelineUnits.Minute:
                        return "";
                    case TimelineUnits.Hour:
						if (tlcdate.baseDate.Minute == 0) return "";
                        if (tlcdate.baseDate.Minute % 5 == 0) return tlcdate.baseDate.Minute.ToString("00");
                        return "";
                    case TimelineUnits.Day:
                        if (tlcdate.baseDate.Hour == 0) return "";
                        return tlcdate.baseDate.Hour.ToString();
                    case TimelineUnits.Month:
                        //if (tlcdate.baseDate.Day == 1) return "";
                        return tlcdate.baseDate.Day.ToString();
                    case TimelineUnits.Year:
                        //if (tlcdate.baseDate.Month == 1) return "";
                        return shortMonthNames[tlcdate.baseDate.Month - 1];
                    case TimelineUnits.Decade:
                        //if (tlcdate.baseDate.Year % 10 == 0) return "";
                        return tlcdate.baseDate.Year.ToString();
                    case TimelineUnits.Century:
                        return tlcdate.Decade.ToString() + "0";
                    default:
                        return "";
                }
			}

		}

        private void AdjustZoomUnit()
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Hour:
                    if (Zoom > hourToDayZoomLimit) ZoomUnit = TimelineUnits.Day;
                    showSubUnitText = (Zoom < hourSubunitLimit) ? true : false;
                    break;

                case TimelineUnits.Day:
                    if (Zoom > dayToMonthZoomLimit) ZoomUnit = TimelineUnits.Month;
                    if (Zoom < hourToDayZoomLimit) ZoomUnit = TimelineUnits.Hour;
                    showSubUnitText = (Zoom < daySubunitLimit) ? true : false;
                    break;

                case TimelineUnits.Month:
                    if (Zoom > monthToYearZoomLimit) ZoomUnit = TimelineUnits.Year;
                    if (Zoom < dayToMonthZoomLimit) ZoomUnit = TimelineUnits.Day;
                    showSubUnitText = (Zoom < monthSubunitLimit) ? true : false;
                    break;

                case TimelineUnits.Year:
                    if (Zoom > yearToDecadeZoomLimit) ZoomUnit = TimelineUnits.Decade;
                    if (Zoom < monthToYearZoomLimit) ZoomUnit = TimelineUnits.Month;
                    showSubUnitText = (Zoom < yearSubunitLimit) ? true : false;
                    break;

                case TimelineUnits.Decade:
                    if (Zoom < yearToDecadeZoomLimit) ZoomUnit = TimelineUnits.Year;
                    showSubUnitText = (Zoom < decadeSubunitLimit) ? true : false;
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
					timelineWidth = info.Width * 0.15f;
                    timelineLeftX = info.Width - timelineWidth;
                    timelineMiddleX = timelineLeftX + timelineWidth / 2;
                    unitMarkX1 = timelineLeftX;
					unitMarkX2 = unitMarkX1 + (timelineWidth * 0.2f);
                    subUnitMarkX1 = timelineLeftX;
					subUnitMarkX2 = subUnitMarkX1 + (timelineWidth * 0.1f);
					unitTextX = timelineLeftX + (timelineWidth * 0.25f);
					subUnitTextX = timelineLeftX + (timelineWidth * 0.15f);
                    halfHeight = info.Height / 2;

                    timelinePaint.Color = Color.SkyBlue.ToSKColor();
                    timelinePaint.StrokeWidth = timelineWidth;
                    unitMarkPaint.Color = Color.Black.ToSKColor();
                    unitMarkPaint.StrokeWidth = 4;
                    unitTextPaint.Color = Color.Black.ToSKColor();
                    unitTextPaint.TextSize = GetTextSizeForWidth(timelineWidth - unitTextX + timelineLeftX);
                    unitTextHalfHeight = unitTextPaint.FontMetrics.CapHeight / 2;
					subUnitMarkPaint.Color = Color.DimGray.ToSKColor();
                    subUnitMarkPaint.StrokeWidth = 2;
					subUnitTextPaint.Color = Color.DimGray.ToSKColor();
                    subUnitTextPaint.TextSize = unitTextPaint.TextSize - 6;
                    subUnitTextHalfHeight = subUnitTextPaint.FontMetrics.CapHeight / 2;

					unitXWidth = theme_portrait.UnitTextPaint.MeasureText("X");
                    subunitXWidth = theme_portrait.SubUnitTextPaint.MeasureText("X");

					hourToDayZoomLimit = (int)(SEC_PER_HOUR / 100);
					dayToMonthZoomLimit = (int)(SEC_PER_DAY / 150);
					monthToYearZoomLimit = (int)(SEC_PER_MONTH / 100);
					yearToDecadeZoomLimit = (int)(SEC_PER_YEAR / 50);
                    
					hourSubunitLimit = (int)(SEC_PER_MINUTE / subUnitTextPaint.TextSize / 0.2);
					daySubunitLimit = (int)(SEC_PER_HOUR / subUnitTextPaint.TextSize);
					monthSubunitLimit = (int)(SEC_PER_DAY / subUnitTextPaint.TextSize);
					yearSubunitLimit = (int)(SEC_PER_MONTH / subUnitTextPaint.TextSize);
					decadeSubunitLimit = (int)(SEC_PER_YEAR / subUnitTextPaint.TextSize);
                }
                else
                {
					timelineHeight = info.Height * 0.15f;
                    timelineBottomY = timelineHeight;
                    timelineMiddleY = timelineHeight / 2;
                    unitMarkY1 = timelineBottomY;
					unitMarkY2 = unitMarkY1 - (timelineHeight * 0.45f);
                    subUnitMarkY1 = timelineBottomY;
					subUnitMarkY2 = subUnitMarkY1 - (timelineHeight * 0.1f);
					unitTextY = timelineBottomY - (timelineHeight * 0.5f);
					subUnitTextY = timelineBottomY - (timelineHeight * 0.15f);
                    halfWidth = info.Width / 2;

					timelinePaint.Color = Color.SkyBlue.ToSKColor();
                    timelinePaint.StrokeWidth = timelineHeight;
                    unitMarkPaint.Color = Color.Black.ToSKColor();
                    unitMarkPaint.StrokeWidth = 4;
                    unitTextPaint.Color = Color.Black.ToSKColor();
					unitTextPaint.TextSize = unitTextY - 10;
                    unitTextHalfHeight = unitTextPaint.FontMetrics.CapHeight / 2;
                    subUnitMarkPaint.Color = Color.DimGray.ToSKColor();
                    subUnitMarkPaint.StrokeWidth = 2;
					subUnitTextPaint.Color = Color.DimGray.ToSKColor();
                    subUnitTextPaint.TextSize = unitTextPaint.TextSize - 6;
                    subUnitTextHalfHeight = subUnitTextPaint.FontMetrics.CapHeight / 2;

                    unitXWidth = theme_landscape.UnitTextPaint.MeasureText("X");
                    subunitXWidth = theme_landscape.SubUnitTextPaint.MeasureText("X");

					hourToDayZoomLimit = (int)(SEC_PER_HOUR / 100);
                    dayToMonthZoomLimit = (int)(SEC_PER_DAY / 150);
                    monthToYearZoomLimit = (int)(SEC_PER_MONTH / 100);
                    yearToDecadeZoomLimit = (int)(SEC_PER_YEAR / 150);

					hourSubunitLimit = (int)(SEC_PER_MINUTE / subunitXWidth * 2);
					daySubunitLimit = (int)(SEC_PER_HOUR / subunitXWidth / 3);
					monthSubunitLimit = (int)(SEC_PER_DAY / subunitXWidth / 3);
					yearSubunitLimit = (int)(SEC_PER_MONTH / subunitXWidth / 4);
					decadeSubunitLimit = (int)(SEC_PER_YEAR / subunitXWidth / 5);
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
            if (width < 40) return 18;
            if (width < 60) return 22;
            if (width < 80) return 26;
            if (width < 100) return 30;
            if (width < 120) return 34;
            if (width < 140) return 38;
            if (width < 160) return 42;
            return 42;
        }
    }
}
