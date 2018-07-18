using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Essentials;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Timeline.Models;
using Timeline.Objects.Timeline;
using Timeline.Objects.TouchTracking;

namespace Timeline.Controls
{
    public partial class TimelineControl : ContentView
    {
        
        #region "Bindable properties"
		public static readonly BindableProperty TimelineProperty = BindableProperty.Create(
            nameof(Timeline),
            typeof(MTimelineInfo),
            typeof(TimelineControl),
            null, BindingMode.OneWay,
            propertyChanged: OnTimelineChanged);
			
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

        public static readonly BindableProperty LongTapCommandProperty = BindableProperty.Create(nameof(LongTapCommand), typeof(ICommand), typeof(TimelineControl), null);

        public ICommand LongTapCommand
        {
            get { return (ICommand)GetValue(LongTapCommandProperty); }
            set { SetValue(LongTapCommandProperty, value); }
        }

        private static void OnTimelineChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (((TimelineControl)bindable).Timeline != null && ((TimelineControl)bindable).Timeline.Events.Count > 0)
                ((TimelineControl)bindable).date = new TimelineDateTime(((TimelineControl)bindable).Timeline.Events[0].StartDate.Year);
            else
                ((TimelineControl)bindable).date = new TimelineDateTime(DateTime.UtcNow);
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
        
		public MTimelineInfo Timeline
        {
			get { return (MTimelineInfo)GetValue(TimelineProperty); }
			set { SetValue(TimelineProperty, value); }
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

        TimelineTheme theme;

        TimelineDateTime date;
        TimelineDateTime unitDate;

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
        float eventsBottomY;
		int laneHeight;
        int lanesOffsetY;

        bool forceInitialize;

		public TimelineControl()
		{
			InitializeComponent();

            theme = new TimelineTheme("");

			gestureRecognizer = new TouchGestureRecognizer();
			gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

            if (Timeline != null && Timeline.Events.Count > 0)
                date = new TimelineDateTime(Timeline.Events[0].StartDate.Year);
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

                        if (eventsBottomY > fullheight && eventsTopY + fullheight - lanesOffsetY > eventsBottomY)
                            lanesOffsetY = (int)eventsTopY + fullheight - (int)eventsBottomY;
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

            Int64 minTicks = date.Ticks - halfWidth * pixeltime;
            Int64 maxTicks = date.Ticks + halfWidth * pixeltime;

            //TIMELINE
            canvas.DrawRect(0, 0, info.Width, timelineHeight, theme.TimelinePaint);

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
            canvas.DrawLine(halfWidth, 0, halfWidth, timelineBottomY, theme.HighlightPaint);

            //EVENTS
            SKRect clipRect = new SKRect(0, timelineBottomY, info.Width, info.Height);
            canvas.ClipRect(clipRect);

			if (this.Timeline != null) DrawTimelineEvents(canvas, minTicks, maxTicks);
        }

        #region "UNITS AND SUBUNITS DRAWING"
        private void DrawUnitsAndSubUnitsBCAC(SKCanvas canvas, Int64 minTicks, Int64 maxTicks)
        {
            float unitPos;

            unitDate.SetDate(1, 1, 1, 0, 0);
            do  //DRAW AC PART
            {
                unitPos = (unitDate.Ticks - minTicks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, theme.UnitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, theme.UnitTextPaint);  //UNIT TEXT

                Int64 fromTicks = unitDate.Ticks;
                unitDate.Add(ZoomUnit);

                //NORMAL SUBUNIT DRAW
                DrawSubUnits(canvas, minTicks, fromTicks, unitDate.Ticks, ZoomUnit - 1);
            } while (unitPos < fullwidth);

            unitDate.SetDate(1, 1, 1, 0, 0);
            do  //DRAW BC PART
            {
                unitPos = (unitDate.Ticks - minTicks) / pixeltime;
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, theme.UnitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, theme.UnitTextPaint);  //UNIT TEXT

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
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, theme.UnitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, theme.UnitTextPaint);  //UNIT TEXT

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
                canvas.DrawLine(unitPos, unitMarkY1, unitPos, unitMarkY2, theme.UnitMarkPaint);   //UNIT MARK
                canvas.DrawText(GetUnitText(unitDate), unitPos, unitTextY, theme.UnitTextPaint);  //UNIT TEXT

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
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, theme.SubUnitMarkPaint);                                //SUBUNIT MARK
                    if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + 3, subUnitTextY, theme.SubUnitTextPaint);      //SUBUNIT TEXT

                    try { subUnitDate.Add(unit, -1); } catch (OverflowException) { break; }
                }
            }
            else
            {
                while (subUnitDate.Ticks < toTicks)
                {
                    subUnitPos = (subUnitDate.Ticks - minTicks) / pixeltime;
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, theme.SubUnitMarkPaint);                                //SUBUNIT MARK
                    if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + 3, subUnitTextY, theme.SubUnitTextPaint);      //SUBUNIT TEXT

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
            foreach (MTimelineEvent e in this.Timeline.Events)
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
			canvas.DrawRect(startX, eventTop, endX - startX, laneHeight - 1, theme.EventPaint);
            if (highlighted)
                theme.EventBorderPaint.Color = Color.Yellow.ToSKColor();
            else
                theme.EventBorderPaint.Color = Color.Black.ToSKColor();
			canvas.DrawRect(startX, eventTop, endX - startX, laneHeight - 1, theme.EventBorderPaint);

            //draw title
            canvas.Save();
            SKRect clipEvent = new SKRect(startX, eventTop, endX, eventTop + laneHeight - 1);
            canvas.ClipRect(clipEvent);
            DrawTextInRect(canvas, e.Title, clipEvent, theme.EventTextPaint);
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
            unitMarkY1 = timelineBottomY;
			unitMarkY2 = unitMarkY1 - (timelineHeight * 0.45f);
            subUnitMarkY1 = timelineBottomY;
			subUnitMarkY2 = subUnitMarkY1 - (timelineHeight * 0.15f);
			unitTextY = timelineBottomY - (timelineHeight * 0.5f);
			subUnitTextY = timelineBottomY - (timelineHeight * 0.2f);
            fullwidth = info.Width;
            fullheight = info.Height;
            halfWidth = info.Width / 2;

            theme.TimelinePaint.StrokeWidth = timelineHeight;
            theme.UnitMarkPaint.StrokeWidth = 4;
			theme.UnitTextPaint.TextSize = unitTextY - 10;
            theme.SubUnitMarkPaint.StrokeWidth = 2;
            theme.SubUnitTextPaint.TextSize = theme.UnitTextPaint.TextSize - 8;
			theme.HighlightPaint.StrokeWidth = 3;

            unitXWidth = theme.UnitTextPaint.MeasureText("X");
            subunitXWidth = theme.SubUnitTextPaint.MeasureText("X");
                
			zoomLimitHourToDay = (int)(SEC_PER_HOUR / 100);
            zoomLimitDayToMonth = (int)(SEC_PER_DAY / 150);
			zoomLimitMonthToYear = (int)(SEC_PER_MONTH / 200);
			zoomLimitYearToDecade = (int)(SEC_PER_YEAR / 150);

			subunitLimitMinute = (int)(SEC_PER_MINUTE / subunitXWidth * 3);
			subunitLimitHour = (int)(SEC_PER_HOUR / subunitXWidth / 2);
			subunitLimitDay = (int)(SEC_PER_DAY / subunitXWidth / 2);
			subunitLimitMonth = (int)(SEC_PER_MONTH / subunitXWidth / 3);
			subunitLimitYear = (int)(SEC_PER_YEAR / subunitXWidth / 4);

            laneHeight = 200;
            eventsTopY = timelineBottomY + 5;
            eventsBottomY = info.Height;
            if (Timeline != null) eventsBottomY = eventsTopY + laneHeight * Timeline.MaxLane;

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
