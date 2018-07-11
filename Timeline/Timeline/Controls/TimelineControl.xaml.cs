using System;
using Xamarin.Forms;
using Xamarin.Essentials;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Timeline.Models;
using Timeline.Objects.Date;
using Timeline.Objects.TouchTracking;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

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

    public class LongTapEventArg
    {
        public float X;
        public float Y;
        public int Lane;
        public Int64 Ticks;

        public LongTapEventArg(float _x, float _y, int _lane, Int64 _ticks)
        {
            X = _x;
            Y = _y;
            Lane = _lane;
            Ticks = _ticks;
        }
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
            (double)100000, BindingMode.OneWay,
            propertyChanged: OnZoomChanged);

        public static readonly BindableProperty ZoomUnitProperty = BindableProperty.Create(
            nameof(ZoomUnit),
            typeof(TimelineUnits),
            typeof(TimelineControl),
            TimelineUnits.Year, BindingMode.OneWay,
            propertyChanged: OnZoomUnitChanged);
	
        public static readonly BindableProperty DateStrProperty = BindableProperty.Create(
            nameof(DateStr),
            typeof(string),
            typeof(TimelineControl),
            "", BindingMode.OneWay);

        public static readonly BindableProperty EventsStrProperty = BindableProperty.Create(
            nameof(EventsStr),
            typeof(string),
            typeof(TimelineControl),
            "", BindingMode.OneWay);

        //public static readonly BindableProperty CommandProperty = BindableProperty.Create<TimelineControl, ICommand>(x => x.Command, null);
        public static readonly BindableProperty LongTapCommandProperty = BindableProperty.Create(nameof(LongTapCommand), typeof(ICommand), typeof(TimelineControl), null);

        public ICommand LongTapCommand
        {
            get { return (ICommand)GetValue(LongTapCommandProperty); }
            set { SetValue(LongTapCommandProperty, value); }
        }

        private static void OnTimeline1Changed(BindableObject bindable, object oldValue, object newValue)
        {
            if (((TimelineControl)bindable).Timeline1 != null && ((TimelineControl)bindable).Timeline1.Events.Count > 0)
                ((TimelineControl)bindable).date = new TimelineDateTime(((TimelineControl)bindable).Timeline1.Events[0].StartDate.Year);
            else
                ((TimelineControl)bindable).date = new TimelineDateTime(DateTime.UtcNow);
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnTimelineColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).forceInitialize = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnUnitMarkColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).forceInitialize = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnUnitTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).forceInitialize = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnSubUnitMarkColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).forceInitialize = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnSubUnitTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
			((TimelineControl)bindable).forceInitialize = true;
            ((TimelineControl)bindable).InvalidateLayout();
        }

		private static void OnHighlightColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).forceInitialize = true;
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

        public string DateStr
        {
            get { return (string)GetValue(DateStrProperty); }
            set { SetValue(DateStrProperty, value); }
        }

        public string EventsStr
        {
            get { return (string)GetValue(EventsStrProperty); }
            set { SetValue(EventsStrProperty, value); }
        }
        #endregion

        string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        TouchGestureRecognizer gestureRecognizer;

        TimelineDateTime date;
        TimelineDateTime unitDate;

        SKPaint timelinePaint;
        SKPaint unitMarkPaint;
        SKPaint unitTextPaint;
        SKPaint subUnitMarkPaint;
        SKPaint subUnitTextPaint;
		SKPaint highlightPaint;
		SKPaint eventPaint;
		SKPaint eventBorderPaint;
		SKPaint eventTextPaint;

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

        float timelineHeight;
        float timelineBottomY;
        float timelineMiddleY;
        float unitMarkY1;
        float unitMarkY2;
        float subUnitMarkY1;
        float subUnitMarkY2;
        float unitTextY;
        float subUnitTextY;
        int fullwidth;
        int fullheight;
        int halfWidth;

		float eventsTopY;
		int laneHeight;
        int lanesOffsetY;

        bool forceInitialize;

		public TimelineControl()
		{
			InitializeComponent();
			gestureRecognizer = new TouchGestureRecognizer();
			gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

            if (Timeline1 != null && Timeline1.Events.Count > 0)
                date = new TimelineDateTime(Timeline1.Events[0].StartDate.Year);
            else
                date = new TimelineDateTime(DateTime.UtcNow);

			unitDate = new TimelineDateTime();
            DateStr = date.DateStr(ZoomUnit);
            EventsStr = "";

			pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
			showSubUnitText = false;
            lanesOffsetY = 0;

            DeviceDisplay.ScreenMetricsChanaged += DeviceDisplay_ScreenMetricsChanged;
            forceInitialize = true;

			timelinePaint = new SKPaint();
			unitMarkPaint = new SKPaint();
			unitTextPaint = new SKPaint();
			subUnitMarkPaint = new SKPaint();
			subUnitTextPaint = new SKPaint();
			highlightPaint = new SKPaint();

			eventPaint = new SKPaint();
			eventPaint.StrokeWidth = 2;
			eventPaint.Color = Color.DarkGray.ToSKColor();
			eventPaint.Style = SKPaintStyle.Fill;

			eventBorderPaint = new SKPaint();
			eventBorderPaint.Color = Color.Black.ToSKColor();
			eventBorderPaint.StrokeWidth = 4;
			eventBorderPaint.Style = SKPaintStyle.Stroke;

			eventTextPaint = new SKPaint();
			eventTextPaint.Color = Color.White.ToSKColor();
			eventTextPaint.TextSize = 32;
		}

        private void DeviceDisplay_ScreenMetricsChanged(ScreenMetricsChanagedEventArgs e)
        {
            forceInitialize = true;
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
                    if (args.InitialRawLocation.Y < timelineBottomY) break;
                    int clickedLane = ((int)args.InitialRawLocation.Y - (int)timelineBottomY - lanesOffsetY) / laneHeight;
                    LongTapEventArg arg = new LongTapEventArg(args.InitialRawLocation.X, args.InitialRawLocation.Y, clickedLane, 0);
                    if (LongTapCommand != null && LongTapCommand.CanExecute(null)) LongTapCommand.Execute(arg);
                    break;

                case TouchGestureType.Pan:
                    //Console.WriteLine("PAN");
                    //X AXIS -- MOVE TIMELINE
                    try
                    {
                        if (Math.Abs(args.Data.Y) < Math.Abs(args.Data.X))
                        {
                            date.AddTicks(-2 * (long)args.Data.X * pixeltime);
                            DateStr = date.DateStr(ZoomUnit - 1);
                        }
                    }
					catch(OverflowException)
                    {
                        if (args.Data.X < 0)
                            date = TimelineDateTime.MaxValue;
                        else
                            date = TimelineDateTime.MinValue;
                    }

                    //Y AXIS -- MOVE EVENT LANES
                    if (Math.Abs(args.Data.X) < Math.Abs(args.Data.Y))
                    {
                        lanesOffsetY += (int)args.Data.Y * 2;
                        if (lanesOffsetY > 0) lanesOffsetY = 0;
                    }

                    canvasView.InvalidateSurface();
                    break;
                    
                case TouchGestureType.Pinch:
					//Console.WriteLine("PINCH - " + args.Data.ToString());
					Zoom -= Zoom * 0.005 * args.Data.X;
                    if (Zoom < 4) Zoom = 4;
					if (Zoom > 2073600) Zoom = 2073600;
                    pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
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
            if(forceInitialize) InitializeParameters(info);

            canvas.Clear();
            SKPaint cp = new SKPaint();
            cp.Color = Color.AliceBlue.ToSKColor();
            canvas.DrawRect(info.Rect, cp);

            Int64 minTicks = date.Ticks - halfWidth * pixeltime;
            Int64 maxTicks = date.Ticks + halfWidth * pixeltime;

            //TIMELINE
            canvas.DrawLine(0, timelineMiddleY, info.Width, timelineMiddleY, timelinePaint);

            //UNITS AND SUBUNITS
            if(minTicks<0 && maxTicks>0)
            {
                DrawUnitsAndSubUnitsBCAC(canvas, minTicks, maxTicks);   //WHEN BCAC CHANGE IS VISIBLE
            }
            else
            {
                if (minTicks > 0) DrawUnitsAndSubUnitsAC(canvas, minTicks, maxTicks);   //PURE AC
                if (maxTicks < 0) DrawUnitsAndSubUnitsBC(canvas, minTicks, maxTicks);   //PURE BC
            }

            //HIGHLIGHTER
            canvas.DrawLine(halfWidth, 0, halfWidth, timelineBottomY, highlightPaint);

            //EVENTS
            SKRect clipRect = new SKRect(0, timelineBottomY, info.Width, info.Height);
            canvas.ClipRect(clipRect);

			if (this.Timeline1 != null) DrawTimelineEvents(canvas, minTicks, maxTicks);
        }

        #region "UNITS AND SUBUNITS DRAWING"
        private void DrawUnitsAndSubUnitsBCAC(SKCanvas canvas, Int64 minTicks, Int64 maxTicks)
        {
            float unitPos;

            unitDate.SetDate(1, 1, 1, 0, 0);
            do  //DRAW AC PART
            {
                unitPos = (unitDate.Ticks - minTicks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, unitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, unitTextPaint);  //UNIT TEXT

                Int64 fromTicks = unitDate.Ticks;
                unitDate.Add(ZoomUnit);

                //NORMAL SUBUNIT DRAW
                DrawSubUnits(canvas, minTicks, fromTicks, unitDate.Ticks, ZoomUnit - 1);
            } while (unitPos < fullwidth);

            unitDate.SetDate(1, 1, 1, 0, 0);
            do  //DRAW BC PART
            {
                unitPos = (unitDate.Ticks - minTicks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, unitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, unitTextPaint);  //UNIT TEXT

                Int64 fromTicks = unitDate.Ticks;
                unitDate.Add(ZoomUnit, -1);

                //NORMAL SUBUNIT DRAW
                DrawSubUnits(canvas, minTicks, fromTicks, unitDate.Ticks, ZoomUnit - 1, true);
            } while (unitPos > 0);
        }

        private void DrawUnitsAndSubUnitsAC(SKCanvas canvas, Int64 minTicks, Int64 maxTicks)
        {
            date.CopyTo(ref unitDate, ZoomUnit);
            while (unitDate.Ticks > minTicks) unitDate.Add(ZoomUnit, -1);

            float unitPos;
            do
            {
                unitPos = (unitDate.Ticks - minTicks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, unitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, unitTextPaint);  //UNIT TEXT

                Int64 fromTicks = unitDate.Ticks;
                try { unitDate.Add(ZoomUnit); }
                catch (OverflowException)
                {
                    //WHEN WE ARE AT MAX
                    DrawSubUnits(canvas, minTicks, fromTicks, BCACDateTime.MaxTicks, ZoomUnit - 1);
                    break;
                }
                //NORMAL SUBUNIT DRAW
                DrawSubUnits(canvas, minTicks, fromTicks, unitDate.Ticks, ZoomUnit - 1);
            } while (unitPos < fullwidth);
        }

        private void DrawUnitsAndSubUnitsBC(SKCanvas canvas, Int64 minTicks, Int64 maxTicks)
        {
            date.CopyTo(ref unitDate, ZoomUnit);
            while (unitDate.Ticks < maxTicks) unitDate.Add(ZoomUnit, 1);

            float unitPos;
            do
            {
                unitPos = (unitDate.Ticks - minTicks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, unitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, unitTextPaint);  //UNIT TEXT

                Int64 fromTicks = unitDate.Ticks;
                try { unitDate.Add(ZoomUnit, -1); }
                catch (OverflowException)
                {
                    //WHEN WE ARE AT MIN
                    DrawSubUnits(canvas, minTicks, fromTicks, BCACDateTime.MinTicks, ZoomUnit - 1, true);
                    break;
                }
                //NORMAL SUBUNIT DRAW
                DrawSubUnits(canvas, minTicks, fromTicks, unitDate.Ticks, ZoomUnit - 1, true);
            } while (unitPos > 0);
        }

        private void DrawSubUnits(SKCanvas canvas, Int64 minTicks, Int64 fromTicks, Int64 toTicks, TimelineUnits unit, bool backwards=false)
        {
            float subUnitPos;
            TimelineDateTime subUnitDate = TimelineDateTime.FromTicks(fromTicks);

            if(backwards)
            {
                while (subUnitDate.Ticks > toTicks)
                {
                    subUnitPos = (subUnitDate.Ticks - minTicks) / pixeltime;
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, subUnitMarkPaint);                                //SUBUNIT MARK
                    if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + 3, subUnitTextY, subUnitTextPaint);      //SUBUNIT TEXT

                    try { subUnitDate.Add(unit, -1); } catch (OverflowException) { break; }
                }
            }
            else
            {
                while (subUnitDate.Ticks < toTicks)
                {
                    subUnitPos = (subUnitDate.Ticks - minTicks) / pixeltime;
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, subUnitMarkPaint);                                //SUBUNIT MARK
                    if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + 3, subUnitTextY, subUnitTextPaint);      //SUBUNIT TEXT

                    try { subUnitDate.Add(unit); } catch (OverflowException) { break; }
                }
            }

        }

        private string GetUnitText(TimelineDateTime tldate)
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return "";
                case TimelineUnits.Hour:
					return tldate.Hour.ToString() + ":00";
                case TimelineUnits.Day:
					return tldate.Day.ToString() + "." + shortMonthNames[tldate.Month - 1];
                case TimelineUnits.Month:
					return shortMonthNames[tldate.Month - 1];
                case TimelineUnits.Year:
					return tldate.YearStr;
                case TimelineUnits.Decade:
                    return tldate.DecadeStr;
                case TimelineUnits.Century:
                    return tldate.CenturyStr;
                default:
                    return "";
            }
        }
        
		private string GetSubUnitText(TimelineDateTime tlcdate)
		{
			switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return "";
                case TimelineUnits.Hour:
					if (tlcdate.Minute == 0) return "";
					if (tlcdate.Minute % 5 == 0) return tlcdate.Minute.ToString("00");
                    return "";
                case TimelineUnits.Day:
					if (tlcdate.Hour == 0) return "";
					return tlcdate.Hour.ToString();
                case TimelineUnits.Month:
					return tlcdate.Day.ToString();
                case TimelineUnits.Year:
					return shortMonthNames[tlcdate.Month - 1];
                case TimelineUnits.Decade:
					return tlcdate.YearStr;
                case TimelineUnits.Century:
                    return tlcdate.DecadeStr;
                default:
                    return "";
            }      
		}
		#endregion

		private void DrawTimelineEvents(SKCanvas canvas, Int64 minTicks, Int64 maxTicks)
		{
            int topLane = -lanesOffsetY / laneHeight;
            EventsStr = "";
            foreach (MTimelineEvent e in this.Timeline1.Events)
			{
                if (e.LaneNumber < topLane) continue;

				if((e.EndDate.Ticks>minTicks)&&(e.StartDate.Ticks<maxTicks))
				{
                    bool isCurrent = e.StartDate.Ticks < date.Ticks && e.EndDate.Ticks > date.Ticks;
                    if(isCurrent) AddToEventsStr(e);
					DrawTimelineEvent(e, canvas, minTicks, isCurrent);
				}
			}
		}

		private void DrawTimelineEvent(MTimelineEvent e, SKCanvas canvas, Int64 minTicks, bool highlighted)
		{
			float startX;
			float endX;
			float eventTop;

			startX = (e.StartDate.Ticks - minTicks) / pixeltime;
			endX = (e.EndDate.Ticks - minTicks) / pixeltime - 1;
			eventTop = eventsTopY + e.LaneNumber * laneHeight + lanesOffsetY;

            if (eventTop > fullheight) return; //OUT OF SCREEN

            //draw event box
			canvas.DrawRect(startX, eventTop, endX - startX, laneHeight - 1, eventPaint);
            if (highlighted)
                eventBorderPaint.Color = Color.Yellow.ToSKColor();
            else
                eventBorderPaint.Color = Color.Black.ToSKColor();
			canvas.DrawRect(startX, eventTop, endX - startX, laneHeight - 1, eventBorderPaint);

            //draw title
            canvas.Save();
            SKRect clipEvent = new SKRect(startX, eventTop, endX, eventTop + laneHeight - 1);
            canvas.ClipRect(clipEvent);
            DrawTextInRect(canvas, e.Title, clipEvent, eventTextPaint);
            canvas.Restore();
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
        private void InitializeParameters(SKImageInfo info)
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
            fullwidth = info.Width;
            fullheight = info.Height;
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

            eventsTopY = timelineBottomY + 5;
            laneHeight = 200;

            forceInitialize = false;
        }

        private void AddToEventsStr(MTimelineEvent e)
        {
            EventsStr += e.StartDate.DateStr(e.StartDate.Precision) + " -- " + e.Title + "\n";
        }

        //Any touch action we simply forward to the gesture recognizer
        protected void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, args.Location.ToSKPoint(), args.RawLocation.ToSKPoint());
        }

        #region "DRAW TEXT IN RECT"
        //https://forums.xamarin.com/discussion/105582/drawtext-multiline
        public class Line
        {
            public string Value { get; set; }

            public float Width { get; set; }
        }

        private void DrawTextInRect(SKCanvas canvas, string text, SKRect area, SKPaint paint)
        {
            float lineHeight = paint.TextSize * 1.2f;
            var lines = SplitLines(text, paint, area.Width);
            var height = lines.Length * lineHeight;

            var y = area.MidY - height / 2;

            foreach (var line in lines)
            {
                y += lineHeight;
                var x = area.MidX - line.Width / 2;
                canvas.DrawText(line.Value, x, y, paint);
            }
        }

        private Line[] SplitLines(string text, SKPaint paint, float maxWidth)
        {
            var spaceWidth = paint.MeasureText(" ");
            var lines = text.Split('\n');

            return lines.SelectMany((line) =>
            {
                var result = new List<Line>();

                var words = line.Split(new[] { " " }, StringSplitOptions.None);

                var lineResult = new StringBuilder();
                float width = 0;
                foreach (var word in words)
                {
                    var wordWidth = paint.MeasureText(word);
                    var wordWithSpaceWidth = wordWidth + spaceWidth;
                    var wordWithSpace = word + " ";

                    if (width + wordWidth > maxWidth)
                    {
                        result.Add(new Line() { Value = lineResult.ToString(), Width = width });
                        lineResult = new StringBuilder(wordWithSpace);
                        width = wordWithSpaceWidth;
                    }
                    else
                    {
                        lineResult.Append(wordWithSpace);
                        width += wordWithSpaceWidth;
                    }
                }

                result.Add(new Line() { Value = lineResult.ToString(), Width = width });

                return result.ToArray();
            }).ToArray();
        }
        #endregion
    }
}
