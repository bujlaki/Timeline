﻿using System;
using Xamarin.Forms;

using TouchTracking;
using SkiaSharp;
using SkiaSharp.Views.Forms;

using Timeline.Models;

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
        Century
    }

    public enum TimelineOrientation
    {
        Portrait,
        Landscape
    }

    public partial class TimelineControl : ContentView
    {
        
        #region "Bindable properties"
		public static readonly BindableProperty Timeline1Property = BindableProperty.Create(
            nameof(Zoom),
            typeof(MTimeline),
            typeof(TimelineControl),
            null, BindingMode.OneWay,
            propertyChanged: OnTimeline1Changed);
		
		public static readonly BindableProperty TimelineColorProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(Color),
            typeof(TimelineControl),
            Color.SkyBlue, BindingMode.OneWay,
			propertyChanged: OnTimelineColorChanged);
        
		public static readonly BindableProperty UnitMarkColorProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(Color),
            typeof(TimelineControl),
            Color.Black, BindingMode.OneWay,
            propertyChanged: OnUnitMarkColorChanged);

		public static readonly BindableProperty UnitTextColorProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(Color),
            typeof(TimelineControl),
            Color.Black, BindingMode.OneWay,
            propertyChanged: OnUnitTextColorChanged);

		public static readonly BindableProperty SubUnitMarkColorProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(Color),
            typeof(TimelineControl),
            Color.DimGray, BindingMode.OneWay,
            propertyChanged: OnSubUnitMarkColorChanged);

		public static readonly BindableProperty SubUnitTextColorProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(Color),
            typeof(TimelineControl),
            Color.DimGray, BindingMode.OneWay,
            propertyChanged: OnSubUnitTextColorChanged);
	
		public static readonly BindableProperty HighlightColorProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(Color),
            typeof(TimelineControl),
            Color.Yellow, BindingMode.OneWay,
            propertyChanged: OnHighlightColorChanged);
		
        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(double),
            typeof(TimelineControl),
            (double)10, BindingMode.OneWay,
            propertyChanged: OnZoomChanged);

        public static readonly BindableProperty ZoomUnitProperty = BindableProperty.Create(
            nameof(ZoomUnit),
            typeof(TimelineUnits),
            typeof(TimelineControl),
            TimelineUnits.Hour, BindingMode.OneWay,
            propertyChanged: OnZoomUnitChanged);

		public static readonly BindableProperty DateProperty = BindableProperty.Create(
            nameof(Date),
            typeof(DateTime),
            typeof(TimelineControl),
            DateTime.UtcNow, BindingMode.TwoWay,
            propertyChanged: OnDateChanged);
		
        public static readonly BindableProperty DateStrProperty = BindableProperty.Create(
            nameof(DateStr),
            typeof(string),
            typeof(TimelineControl),
            "", BindingMode.OneWay);

		private static void OnTimeline1Changed(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnTimelineColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).initialOrientationCheck = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnUnitMarkColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).initialOrientationCheck = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnUnitTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).initialOrientationCheck = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnSubUnitMarkColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).initialOrientationCheck = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnSubUnitTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).initialOrientationCheck = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnHighlightColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).initialOrientationCheck = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

        private static void OnZoomChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

        private static void OnZoomUnitChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).date.BaseDate = (DateTime)newValue;
			((TimelineControl)bindable).InvalidateLayout();
        }
        
		public MTimeline Timeline1
        {
			get { return (MTimeline)GetValue(Timeline1Property); }
			set { SetValue(Timeline1Property, value); }
        }

		public Color TimelineColor
        {
            get { return (Color)GetValue(TimelineColorProperty); }
			set { SetValue(TimelineColorProperty, value); }
        }

		public Color UnitMarkColor
        {
			get { return (Color)GetValue(UnitMarkColorProperty); }
			set { SetValue(UnitMarkColorProperty, value); }
        }

		public Color UnitTextColor
        {
			get { return (Color)GetValue(UnitTextColorProperty); }
			set { SetValue(UnitTextColorProperty, value); }
        }
        
		public Color SubUnitMarkColor
        {
			get { return (Color)GetValue(SubUnitMarkColorProperty); }
			set { SetValue(SubUnitMarkColorProperty, value); }
        }

		public Color SubUnitTextColor
        {
			get { return (Color)GetValue(SubUnitTextColorProperty); }
			set { SetValue(SubUnitTextColorProperty, value); }
        }

		public Color HighlightColor
        {
			get { return (Color)GetValue(HighlightColorProperty); }
			set { SetValue(HighlightColorProperty, value); }
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
        
		public DateTime Date
        {
            get { return (DateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
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

        TimelineDate date;
        TimelineDate unitDate;
        TimelineDate subUnitDate;

        SKPaint timelinePaint;
        SKPaint unitMarkPaint;
        SKPaint unitTextPaint;
        SKPaint subUnitMarkPaint;
        SKPaint subUnitTextPaint;
		SKPaint highlightPaint;

        Int64 pixeltime;
        float unitXWidth;
        float subunitXWidth;
        bool showSubUnitText;

        int zoomLimitHourToDay;
        int zoomLimitDayToMonth;
        int zoomLimitMonthToYear;
        int zoomLimitYearToDecade;

        int subunitLimitMinute;
        int subunitLimitHour;
        int subunitLimitDay;
        int subunitLimitMonth;
        int subunitLimitYear;

		const int SEC_PER_MINUTE = 60;
		const int SEC_PER_HOUR = 3600;
		const int SEC_PER_DAY = 86400;
		const int SEC_PER_MONTH = 2592000;
		const int SEC_PER_YEAR = 31104000;

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

		public TimelineControl()
		{
			InitializeComponent();
			gestureRecognizer = new TouchGestureRecognizer();
			gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

			date = new TimelineDate(DateTime.UtcNow);
			unitDate = new TimelineDate(DateTime.UtcNow);
			subUnitDate = new TimelineDate(DateTime.UtcNow);
			DateStr = date.DateStr(ZoomUnit);

			pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
			showSubUnitText = false;

			initialOrientationCheck = true;

			timelinePaint = new SKPaint();
			unitMarkPaint = new SKPaint();
			unitTextPaint = new SKPaint();
			subUnitMarkPaint = new SKPaint();
			subUnitTextPaint = new SKPaint();
			highlightPaint = new SKPaint();
		}

        void GestureRecognizer_OnGestureRecognized(object sender, TouchGestureEventArgs args)
        {
            switch (args.Type)
            {
                case TouchGestureType.Tap:
                    //Console.WriteLine("TAP");
                    break;

                case TouchGestureType.LongTap:
                    //Console.WriteLine("LONGTAP");
                    break;

                case TouchGestureType.Pan:
					//Console.WriteLine("PAN");
					date.BaseDate = date.BaseDate.AddTicks(-2 * (long)args.Data.X * pixeltime);
                    DateStr = date.DateStr(ZoomUnit-1);
                    canvasView.InvalidateSurface();
                    break;
                    
                case TouchGestureType.Pinch:
					//Console.WriteLine("PINCH - " + args.Data.ToString());
					Zoom -= Zoom * 0.005 * args.Data.X;
                    if (Zoom < 4) Zoom = 4;
					if (Zoom > 2073600) Zoom = 2073600;
                    pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
                    //Console.WriteLine("Zoom: " + Zoom.ToString() + "  pixeltime: " + pixeltime.ToString());
                    AdjustZoomUnit();
                    DateStr = date.DateStr(ZoomUnit - 1);
                    canvasView.InvalidateSurface();
                    break;

                case TouchGestureType.Swipe:
                    //Console.WriteLine("SWIPE");
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
            
            //TIMELINE
            canvas.DrawLine(0, timelineMiddleY, info.Width, timelineMiddleY, timelinePaint);
            //UNITS AND SUBUNITS
            DrawUnitsAndSubUnits(canvas);
            //HIGHLIGHTER
            canvas.DrawLine(halfWidth, 0, halfWidth, timelineBottomY, highlightPaint);

			//EVENTS
			if (this.Timeline1 != null) DrawTimelineEvents(canvas);
        }

		#region "UNITS AND SUBUNITS DRAWING"
		private void DrawUnitsAndSubUnits(SKCanvas canvas)
        {
            float unitPos;
            float subUnitPos;

			DateTime minDate = new DateTime(date.BaseDate.Ticks - halfWidth * pixeltime);
			DateTime maxDate = new DateTime(date.BaseDate.Ticks + halfWidth * pixeltime);

            date.CopyTo(ref unitDate, ZoomUnit);

			while (unitDate.BaseDate.Ticks > minDate.Ticks) unitDate.Add(ZoomUnit, -1);

			while (unitDate.BaseDate.Ticks < maxDate.Ticks)
            {
				unitPos = (unitDate.BaseDate.Ticks - minDate.Ticks) / pixeltime;

                //UNIT MARK
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, unitMarkPaint);
                //UNIT TEXT
				canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, unitTextPaint);

                unitDate.CopyTo(ref subUnitDate, ZoomUnit - 1);
				unitDate.Add(ZoomUnit);
                
				while (subUnitDate.BaseDate.Ticks < unitDate.BaseDate.Ticks && subUnitDate.BaseDate.Ticks < maxDate.Ticks)
                {
					subUnitPos = (subUnitDate.BaseDate.Ticks - minDate.Ticks) / pixeltime;
                    
                    //SUBUNIT MARK
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, subUnitMarkPaint);
                    
					//SUBUNIT TEXT
					if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + 3, subUnitTextY, subUnitTextPaint);

                    subUnitDate.Add(ZoomUnit - 1);
                }
            }
        }

        private string GetUnitText(TimelineDate tlcdate)
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return "";
                case TimelineUnits.Hour:
					return tlcdate.BaseDate.Hour.ToString() + ":00";
                case TimelineUnits.Day:
					return tlcdate.BaseDate.Day.ToString() + "." + shortMonthNames[tlcdate.BaseDate.Month - 1];
                case TimelineUnits.Month:
					return shortMonthNames[tlcdate.BaseDate.Month - 1];
                case TimelineUnits.Year:
					return tlcdate.BaseDate.Year.ToString();
                case TimelineUnits.Decade:
                    return tlcdate.Decade.ToString() + "0";
                case TimelineUnits.Century:
                    return tlcdate.Century.ToString() + "00";
                default:
                    return "";
            }
        }
        
		private string GetSubUnitText(TimelineDate tlcdate)
		{
			switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return "";
                case TimelineUnits.Hour:
					if (tlcdate.BaseDate.Minute == 0) return "";
					if (tlcdate.BaseDate.Minute % 5 == 0) return tlcdate.BaseDate.Minute.ToString("00");
                    return "";
                case TimelineUnits.Day:
					if (tlcdate.BaseDate.Hour == 0) return "";
					return tlcdate.BaseDate.Hour.ToString();
                case TimelineUnits.Month:
					return tlcdate.BaseDate.Day.ToString();
                case TimelineUnits.Year:
					return shortMonthNames[tlcdate.BaseDate.Month - 1];
                case TimelineUnits.Decade:
					return tlcdate.BaseDate.Year.ToString();
                case TimelineUnits.Century:
                    return tlcdate.Decade.ToString() + "0";
                default:
                    return "";
            }      
		}
		#endregion

		private void DrawTimelineEvents(SKCanvas canvas)
		{
			DateTime minDate = new DateTime(date.BaseDate.Ticks - halfWidth * pixeltime);
			DateTime maxDate = new DateTime(date.BaseDate.Ticks + halfWidth * pixeltime);
            
			foreach(MTimelineEvent e in this.Timeline1.Events)
			{
				if((e.StartDate.BaseDate.Ticks>minDate.Ticks)&&(e.StartDate.BaseDate.Ticks<maxDate.Ticks))
				{
					DrawTimelineEvent(e, canvas, minDate);
				}
			}
		}

		private void DrawTimelineEvent(MTimelineEvent e, SKCanvas canvas, DateTime minDate)
		{
			float eventX;
			eventX = (e.StartDate.BaseDate.Ticks - minDate.Ticks) / pixeltime;
			canvas.DrawRect(eventX, timelineBottomY, 50, 20, timelinePaint);
		}

		private void AdjustZoomUnit()
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Hour:
                    if (Zoom > zoomLimitHourToDay) ZoomUnit = TimelineUnits.Day;
                    break;
                case TimelineUnits.Day:
                    if (Zoom > zoomLimitDayToMonth) ZoomUnit = TimelineUnits.Month;
					if (Zoom < zoomLimitHourToDay) ZoomUnit = TimelineUnits.Hour;
                    break;
                case TimelineUnits.Month:
                    if (Zoom > zoomLimitMonthToYear) ZoomUnit = TimelineUnits.Year;
					if (Zoom < zoomLimitDayToMonth) ZoomUnit = TimelineUnits.Day;
                    break;
                case TimelineUnits.Year:
                    if (Zoom > zoomLimitYearToDecade) ZoomUnit = TimelineUnits.Decade;
					if (Zoom < zoomLimitMonthToYear) ZoomUnit = TimelineUnits.Month;
                    break;
                case TimelineUnits.Decade:
					if (Zoom < zoomLimitYearToDecade) ZoomUnit = TimelineUnits.Year;
                    break;
            }

			switch (this.ZoomUnit)
            {
                case TimelineUnits.Hour:
                    showSubUnitText = (Zoom < subunitLimitMinute) ? true : false;
                    break;
                case TimelineUnits.Day:
                    showSubUnitText = (Zoom < subunitLimitHour) ? true : false;
                    break;
                case TimelineUnits.Month:
                    showSubUnitText = (Zoom < subunitLimitDay) ? true : false;
                    break;
                case TimelineUnits.Year:
                    showSubUnitText = (Zoom < subunitLimitMonth) ? true : false;
                    break;
                case TimelineUnits.Decade:
                    showSubUnitText = (Zoom < subunitLimitYear) ? true : false;
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
				timelineHeight = info.Height * 0.10f;
				if (timelineHeight < 100) timelineHeight = 100;
                timelineBottomY = timelineHeight;
                timelineMiddleY = timelineHeight / 2;
                unitMarkY1 = timelineBottomY;
				unitMarkY2 = unitMarkY1 - (timelineHeight * 0.45f);
                subUnitMarkY1 = timelineBottomY;
				subUnitMarkY2 = subUnitMarkY1 - (timelineHeight * 0.15f);
				unitTextY = timelineBottomY - (timelineHeight * 0.5f);
				subUnitTextY = timelineBottomY - (timelineHeight * 0.2f);
                halfWidth = info.Width / 2;

				timelinePaint.Color = this.TimelineColor.ToSKColor();
                timelinePaint.StrokeWidth = timelineHeight;
                unitMarkPaint.Color = this.UnitMarkColor.ToSKColor();
                unitMarkPaint.StrokeWidth = 4;
                unitTextPaint.Color = this.UnitTextColor.ToSKColor();
				unitTextPaint.TextSize = unitTextY - 10;
                subUnitMarkPaint.Color = this.SubUnitMarkColor.ToSKColor();
                subUnitMarkPaint.StrokeWidth = 2;
				subUnitTextPaint.Color = this.SubUnitTextColor.ToSKColor();
                subUnitTextPaint.TextSize = unitTextPaint.TextSize - 8;
				highlightPaint.Color = this.HighlightColor.ToSKColor();
				highlightPaint.StrokeWidth = 3;

                unitXWidth = unitTextPaint.MeasureText("X");
                subunitXWidth = subUnitTextPaint.MeasureText("X");
                
				zoomLimitHourToDay = (int)(SEC_PER_HOUR / 100);
                zoomLimitDayToMonth = (int)(SEC_PER_DAY / 150);
				zoomLimitMonthToYear = (int)(SEC_PER_MONTH / 200);
				zoomLimitYearToDecade = (int)(SEC_PER_YEAR / 150);

				subunitLimitMinute = (int)(SEC_PER_MINUTE / subunitXWidth * 3);
				subunitLimitHour = (int)(SEC_PER_HOUR / subunitXWidth / 2);
				subunitLimitDay = (int)(SEC_PER_DAY / subunitXWidth / 2);
				subunitLimitMonth = (int)(SEC_PER_MONTH / subunitXWidth / 3);
				subunitLimitYear = (int)(SEC_PER_YEAR / subunitXWidth / 4);

                initialOrientationCheck = false;
            }
        }

        //Any touch action we simply forward to the gesture recognizer
        protected void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, args.Location.ToSKPoint());
        }
        
    }
}
