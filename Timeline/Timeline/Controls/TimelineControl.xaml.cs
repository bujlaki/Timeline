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
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Timeline.Controls
{
    public partial class TimelineControl : ContentView
    {

        #region "Bindable properties"

        public static readonly BindableProperty DateProperty = BindableProperty.Create(
            nameof(Date),
            typeof(TimelineDateTime),
            typeof(TimelineControl),
            null, BindingMode.TwoWay,
            propertyChanged: OnDateChanged);
        private static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }
        public TimelineDateTime Date
        {
            get { return (TimelineDateTime)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ObservableCollection<MTimelineEvent>),
            typeof(TimelineControl),
            new ObservableCollection<MTimelineEvent>(), BindingMode.TwoWay,
            propertyChanged: OnItemsSourceChanged);
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TimelineControl tlc = (TimelineControl)bindable;

            //signup to CollectionChanged, so we can update the Canvas when an event is added
            if (oldValue != null) ((ObservableCollection<MTimelineEvent>)oldValue).CollectionChanged -= tlc.OnItemsSourceCollectionChanged;
            if (newValue != null) ((ObservableCollection<MTimelineEvent>)newValue).CollectionChanged += tlc.OnItemsSourceCollectionChanged;

            if (tlc.ItemsSource.Count > 0)
                tlc.Date = new TimelineDateTime(tlc.ItemsSource[0].StartDate.Year);
            else
                tlc.Date = new TimelineDateTime(DateTime.UtcNow);
            tlc.canvasView.InvalidateSurface();
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            canvasView.InvalidateSurface();
        }

        public ObservableCollection<MTimelineEvent> ItemsSource
        {
            get { return (ObservableCollection<MTimelineEvent>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty LaneCountProperty = BindableProperty.Create(
            nameof(Timeline),
            typeof(int),
            typeof(TimelineControl),
            0, BindingMode.TwoWay,
            propertyChanged: OnLaneCountChanged);
        private static void OnLaneCountChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (((TimelineControl)bindable).LaneCount > 0)
                ((TimelineControl)bindable).eventsBottomY = ((TimelineControl)bindable).eventsTopY + ((TimelineControl)bindable).laneHeight * ((TimelineControl)bindable).LaneCount;

            ((TimelineControl)bindable).InvalidateLayout();
        }
        public int LaneCount
        {
            get { return (int)GetValue(LaneCountProperty); }
            set { SetValue(LaneCountProperty, value); }
        }

        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(double),
            typeof(TimelineControl),
            (double)100000, BindingMode.OneWay,
            propertyChanged: OnZoomChanged);
        private static void OnZoomChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }
        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly BindableProperty ZoomUnitProperty = BindableProperty.Create(
            nameof(ZoomUnit),
            typeof(TimelineUnits),
            typeof(TimelineControl),
            TimelineUnits.Year, BindingMode.TwoWay,
            propertyChanged: OnZoomUnitChanged);
        private static void OnZoomUnitChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }
        public TimelineUnits ZoomUnit
        {
            get { return (TimelineUnits)GetValue(ZoomUnitProperty); }
            set { SetValue(ZoomUnitProperty, value); }
        }

        public static readonly BindableProperty PixeltimeProperty = BindableProperty.Create(
            nameof(Pixeltime),
            typeof(Int64),
            typeof(TimelineControl),
            (Int64)0, BindingMode.OneWayToSource);
        public Int64 Pixeltime
        {
            get { return (Int64)GetValue(PixeltimeProperty); }
            set { SetValue(PixeltimeProperty, value); }
        }

        public static readonly BindableProperty DateStrProperty = BindableProperty.Create(
            nameof(DateStr),
            typeof(string),
            typeof(TimelineControl),
            "", BindingMode.OneWay);
        public string DateStr
        {
            get { return (string)GetValue(DateStrProperty); }
            set { SetValue(DateStrProperty, value); }
        }

        public static readonly BindableProperty EventsStrProperty = BindableProperty.Create(
            nameof(EventsStr),
            typeof(string),
            typeof(TimelineControl),
            "", BindingMode.OneWay);
        public string EventsStr
        {
            get { return (string)GetValue(EventsStrProperty); }
            set { SetValue(EventsStrProperty, value); }
        }

        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(TimelineControl),
            null);
        public ICommand TapCommand
        {
            get { return (ICommand)GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }

        public static readonly BindableProperty LongTapCommandProperty = BindableProperty.Create(
            nameof(LongTapCommand), 
            typeof(ICommand), 
            typeof(TimelineControl), 
            null);
        public ICommand LongTapCommand
        {
            get { return (ICommand)GetValue(LongTapCommandProperty); }
            set { SetValue(LongTapCommandProperty, value); }
        }

        #endregion

        string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        TouchGestureRecognizer gestureRecognizer;

        TimelineTheme theme;

        //TimelineDateTime date;
        TimelineDateTime unitDate;

        //Int64 pixeltime;
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

            if (ItemsSource.Count > 0)
                Date = new TimelineDateTime(ItemsSource[0].StartDate.Year);
            else
                Date = new TimelineDateTime(DateTime.UtcNow);

			unitDate = new TimelineDateTime();
            DateStr = Date.DateStr(ZoomUnit);
            EventsStr = "";

			Pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
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
            int clickedLane;
            Int64 clickedTicks;

            switch (args.Type)
            {
                case TouchGestureType.Tap:
                    //Console.WriteLine("TAP");
                    if (args.InitialRawLocation.Y < timelineBottomY) break;
                    clickedLane = ((int)args.InitialRawLocation.Y - (int)timelineBottomY - lanesOffsetY) / laneHeight;
                    clickedTicks = Date.Ticks + ((int)args.InitialRawLocation.X - halfWidth) * Pixeltime;
                    TapEventArg taparg = new TapEventArg(args.InitialRawLocation.X, args.InitialRawLocation.Y, clickedLane, clickedTicks, ZoomUnit);
                    if (TapCommand != null && TapCommand.CanExecute(null))
                    {
                        TapCommand.Execute(taparg);
                    }
                    break;

                case TouchGestureType.LongTap:
                    //Console.WriteLine("LONGTAP");
                    if (args.InitialRawLocation.Y < timelineBottomY) break;
                    clickedLane = ((int)args.InitialRawLocation.Y - (int)timelineBottomY - lanesOffsetY) / laneHeight;
                    clickedTicks = Date.Ticks + ((int)args.InitialRawLocation.X - halfWidth) * Pixeltime;
                    TapEventArg longtaparg = new TapEventArg(args.InitialRawLocation.X, args.InitialRawLocation.Y, clickedLane, clickedTicks, ZoomUnit);
                    if (LongTapCommand != null && LongTapCommand.CanExecute(null))
                    {
                        LongTapCommand.Execute(longtaparg);
                    }
                    break;

                case TouchGestureType.Pan:
                    //Console.WriteLine("PAN");
                    //X AXIS -- MOVE TIMELINE
                    try
                    {
                        if (Math.Abs(args.Data.Y) < Math.Abs(args.Data.X))
                        {
                            Date.AddTicks(-2 * (long)args.Data.X * Pixeltime);
                            DateStr = Date.DateStr(ZoomUnit - 1);
                        }
                    }
					catch(OverflowException)
                    {
                        if (args.Data.X < 0)
                            Date = TimelineDateTime.MaxValue;
                        else
                            Date = TimelineDateTime.MinValue;
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
                    Pixeltime = (long)(Zoom * TimeSpan.TicksPerSecond);
                    AdjustZoomUnit();
                    DateStr = Date.DateStr(ZoomUnit - 1);
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

            Int64 minTicks = Date.Ticks - halfWidth * Pixeltime;
            Int64 maxTicks = Date.Ticks + halfWidth * Pixeltime;

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

			if (ItemsSource.Count > 0) DrawTimelineEvents(canvas, minTicks, maxTicks);
        }

        #region "UNITS AND SUBUNITS DRAWING"
        private void DrawUnitsAndSubUnitsBCAC(SKCanvas canvas, Int64 minTicks, Int64 maxTicks)
        {
            float unitPos;

            unitDate.SetDate(1, 1, 1, 0, 0);
            do  //DRAW AC PART
            {
                unitPos = (unitDate.Ticks - minTicks) / Pixeltime;
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
                unitPos = (unitDate.Ticks - minTicks) / Pixeltime;
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
            Date.CopyTo(ref unitDate, ZoomUnit);
            while (unitDate.Ticks > minTicks) unitDate.Add(ZoomUnit, -1);

            float unitPos;
            do
            {
                unitPos = (unitDate.Ticks - minTicks) / Pixeltime;
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
            Date.CopyTo(ref unitDate, ZoomUnit);
            while (unitDate.Ticks < maxTicks) unitDate.Add(ZoomUnit, 1);

            float unitPos;
            do
            {
                unitPos = (unitDate.Ticks - minTicks) / Pixeltime;
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
                    subUnitPos = (subUnitDate.Ticks - minTicks) / Pixeltime;
                    canvas.DrawLine(subUnitPos, subUnitMarkY1, subUnitPos, subUnitMarkY2, theme.SubUnitMarkPaint);                                //SUBUNIT MARK
                    if (showSubUnitText) canvas.DrawText(GetSubUnitText(subUnitDate), subUnitPos + 3, subUnitTextY, theme.SubUnitTextPaint);      //SUBUNIT TEXT

                    try { subUnitDate.Add(unit, -1); } catch (OverflowException) { break; }
                }
            }
            else
            {
                while (subUnitDate.Ticks < toTicks)
                {
                    subUnitPos = (subUnitDate.Ticks - minTicks) / Pixeltime;
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
            foreach (MTimelineEvent e in this.ItemsSource)
			{
                if (e.LaneNumber < topLane) continue;

				if((e.EndDate.Ticks>minTicks)&&(e.StartDate.Ticks<maxTicks))
				{
                    bool isCurrent = e.StartDate.Ticks < Date.Ticks && e.EndDate.Ticks > Date.Ticks;
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

			startX = (e.StartDate.Ticks - minTicks) / Pixeltime;
			endX = (e.EndDate.Ticks - minTicks) / Pixeltime - 1;
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
            if (LaneCount > 0) eventsBottomY = eventsTopY + laneHeight * LaneCount;

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
