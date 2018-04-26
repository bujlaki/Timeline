using System;
using Xamarin.Forms;

using TouchTracking;
using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    public enum TimelineUnits {
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

    public enum TimelineOrientation {
        Portrait,
        Landscape
    }

    public partial class TimelineControl : ContentView
    {
        
#region "Bindable properties"
        public static readonly BindableProperty OffsetProperty = BindableProperty.Create(
            nameof(Offset),
            typeof(long),
            typeof(TimelineControl),
            (long)-2018 * 100, BindingMode.OneWay,
            propertyChanged: OnOffsetChanged);

        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(long),
            typeof(TimelineControl),
            (long)100, BindingMode.OneWay,
            propertyChanged: OnZoomChanged);

        public static readonly BindableProperty ZoomUnitProperty = BindableProperty.Create(
            nameof(ZoomUnit),
            typeof(TimelineUnits),
            typeof(TimelineControl),
            TimelineUnits.Year, BindingMode.OneWay,
            propertyChanged: OnZoomUnitChanged);
        
        public static readonly BindableProperty StartYearProperty = BindableProperty.Create(
            nameof(StartYear), 
            typeof(long), 
            typeof(TimelineControl), 
            (long)1, BindingMode.OneWay,
            propertyChanged: OnStartYearChanged);

        public static readonly BindableProperty EndYearProperty = BindableProperty.Create(
            nameof(EndYear),
            typeof(long),
            typeof(TimelineControl),
            (long)DateTime.Now.Year, BindingMode.OneWay,
            propertyChanged: OnEndYearChanged);
      
        private static void OnOffsetChanged(BindableObject bindable, object oldValue, object newValue)
        {
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

        private static void OnStartYearChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

        private static void OnEndYearChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

        public long Offset
        {
            get { return (long)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public long Zoom
        {
            get { return (long)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); AdjustZoomUnit(); }
        }

        public TimelineUnits ZoomUnit
        {
            get { return (TimelineUnits)GetValue(ZoomUnitProperty); }
            set { SetValue(ZoomUnitProperty, value); }
        }

        public long StartYear
        {
            get { return (long)GetValue(StartYearProperty); }
            set { SetValue(StartYearProperty, value); }
        }

        public long EndYear
        {
            get { return (long)GetValue(EndYearProperty); }
            set { SetValue(EndYearProperty, value); }
        }
#endregion

        private string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        private TouchGestureRecognizer gestureRecognizer;
        private TimelineOrientation orientation;
        private TimelineControlTheme theme;
        private int timelineLeftPos;

        int unitMarkX1;
        int unitMarkX2;
        int subUnitMarkX1;
        int subUnitMarkX2;
        int unitTextX;

        public TimelineControl()
        {
            InitializeComponent();
            gestureRecognizer = new TouchGestureRecognizer();
            gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

            theme = new TimelineControlTheme();

            CheckOrientation();
        }

        void GestureRecognizer_OnGestureRecognized(object sender, TouchGestureEventArgs args)
        {
            switch(args.Type)
            {
                case TouchGestureType.Tap:
                    Console.WriteLine("TAP");
                    break;

                case TouchGestureType.LongTap:
                    Console.WriteLine("LONGTAP");
                    break;

                case TouchGestureType.Pan:
                    Console.WriteLine("PAN");
                    Offset += (long)args.Data.Y * 2;
                    canvasView.InvalidateSurface();
                    break;

                case TouchGestureType.Pinch:
                    Console.WriteLine("PINCH - " + args.Data.ToString());
                    long oldZoom = Zoom;
                    Zoom += (long)args.Data.Y;
                    Offset = Offset * Zoom / oldZoom;
                    Console.WriteLine("zoom: " + Zoom.ToString() + " --- offset: " + Offset.ToString());
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
            timelineLeftPos = info.Width - theme.TimelineWidth;
            int lineMiddleX = timelineLeftPos + theme.TimelineWidth / 2;
            if (orientation == TimelineOrientation.Portrait)
            {
                canvas.DrawLine(lineMiddleX, 0, lineMiddleX, info.Height, theme.TimelinePaint);
            } else {
                canvas.DrawLine(0, 25, info.Width, 25, theme.TimelinePaint);
            }

            //DRAW UNITS
            unitMarkX1 = timelineLeftPos + theme.UnitMarkOffset;
            unitMarkX2 = unitMarkX1 + theme.UnitMarkLength;
            subUnitMarkX1 = timelineLeftPos + theme.SubUnitMarkOffset;
            subUnitMarkX2 = subUnitMarkX1 + theme.SubUnitMarkLength;
            unitTextX = timelineLeftPos + theme.UnitTextOffset;

            switch(this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    break;

                case TimelineUnits.Hour:
                    break;

                case TimelineUnits.Day:
                    break;

                case TimelineUnits.Month:
                    DrawMonths(info, canvas);
                    break;

                case TimelineUnits.Year:
                    DrawYears(info, canvas);
                    break;

                case TimelineUnits.Decade:
                    DrawDecades(info, canvas);
                    break;

                case TimelineUnits.KYear:
                    break;

                case TimelineUnits.MYear:
                    break;
            }
        }

        private void DrawDays(SKImageInfo info, SKCanvas canvas)
        {
            long firstVisibleUnit = -this.Offset / this.Zoom;
            long lastVisibleUnit = (-this.Offset + info.Height) / this.Zoom + 1;

            //main unit data - MONTH
            long unitPos = firstVisibleUnit * this.Zoom + this.Offset;
            for (long i = firstVisibleUnit; i < lastVisibleUnit; i++)
            {
                int year = (int)(i / 12);
                int month = (int)(i % 12 + 1);
                canvas.DrawLine(unitMarkX1, unitPos, unitMarkX2, unitPos, theme.UnitMarkPaint);

                string unitText1 = (month == 1) ? year.ToString() + " Jan" : shortMonthNames[month - 1];
                canvas.DrawText(unitText1, unitTextX, unitPos + theme.UnitTextPaint.TextSize, theme.UnitTextPaint);

                //optional subunit data - DAY
                if (Zoom > 500)
                {
                    int days = DateTime.DaysInMonth(year, month);
                    float subunitStep = (float)Zoom / days;
                    for (int day = 0; day < days; day++)
                    {
                        float subunitPos = unitPos + day * subunitStep;
                        canvas.DrawLine(subUnitMarkX1, subunitPos, subUnitMarkX2, subunitPos, theme.UnitMarkPaint);
                    }
                }
                unitPos += this.Zoom;
            }
        }

        private void DrawMonths(SKImageInfo info, SKCanvas canvas)
        {
            long firstVisibleUnit = -this.Offset / this.Zoom;
            long lastVisibleUnit = (-this.Offset + info.Height) / this.Zoom + 1;

            //main unit data - MONTH
            long unitPos = firstVisibleUnit * this.Zoom + this.Offset;
            for (long i = firstVisibleUnit; i < lastVisibleUnit; i++)
            {
                int year = (int)(i / 12);
                int month = (int)(i % 12 + 1);
                canvas.DrawLine(unitMarkX1, unitPos, unitMarkX2, unitPos, theme.UnitMarkPaint);

                string unitText1 = (month == 1) ? year.ToString() + " Jan" : shortMonthNames[month - 1];
                canvas.DrawText(unitText1, unitTextX, unitPos + theme.UnitTextPaint.TextSize, theme.UnitTextPaint);

                //optional subunit data - DAY
                if(Zoom>500)
                {
                    int days = DateTime.DaysInMonth(year, month);
                    float subunitStep = (float)Zoom / days;
                    for (int day = 0; day < days; day++)
                    {
                        float subunitPos = unitPos + day * subunitStep;
                        canvas.DrawLine(subUnitMarkX1, subunitPos, subUnitMarkX2, subunitPos, theme.UnitMarkPaint);
                    }
                }
                unitPos += this.Zoom;
            }
        }

        private void DrawYears(SKImageInfo info, SKCanvas canvas)
        {
            int firstVisibleUnit = (int)(-this.Offset / this.Zoom);
            int lastVisibleUnit = (int)((-this.Offset + info.Height) / this.Zoom + 1);

            //main unit data - YEAR
            long unitPos = firstVisibleUnit * this.Zoom + this.Offset;
            for (int i = firstVisibleUnit; i < lastVisibleUnit; i++)
            {
                canvas.DrawLine(unitMarkX1, unitPos, unitMarkX2, unitPos, theme.UnitMarkPaint);
                canvas.DrawText(i.ToString(), unitTextX, unitPos + theme.UnitTextPaint.TextSize, theme.UnitTextPaint);

                //optional subunit data - MONTH
                if (Zoom > 350)
                {
                    float subunitStep = (float)Zoom / 12;
                    for (int month = 0; month < 12; month++)
                    {
                        float subunitPos = unitPos + month * subunitStep;
                        canvas.DrawLine(subUnitMarkX1, subunitPos, subUnitMarkX2, subunitPos, theme.UnitMarkPaint);
                    }
                }

                unitPos += this.Zoom;
            }

        }

        private void DrawDecades(SKImageInfo info, SKCanvas canvas)
        {
            int firstVisibleDecade = (int)(-this.Offset / this.Zoom + 1);
            int lastVisibleDecade = (int)((-this.Offset + info.Height) / this.Zoom + 1);

            SKPath path = new SKPath();
            path.MoveTo(info.Width - 45, 0);
            path.LineTo(info.Width - 45, info.Height);

            SKPaint paintText = new SKPaint();
            paintText.Color = Color.Black.ToSKColor();
            paintText.TextSize = 32;

            long decadePos = firstVisibleDecade * this.Zoom + this.Offset;
            for (int i = firstVisibleDecade; i < lastVisibleDecade; i++)
            {
                canvas.DrawTextOnPath((i*10).ToString(), path, decadePos, 0, paintText);
                decadePos += this.Zoom;
            }
        }

        private void AdjustZoomUnit()
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    break;

                case TimelineUnits.Hour:
                    break;

                case TimelineUnits.Day:
                    break;

                case TimelineUnits.Month:
                    if(Zoom<50)
                    {
                        ZoomUnit = TimelineUnits.Year;
                        Zoom = 600;
                        Offset = Offset / 12;
                    }
                    break;

                case TimelineUnits.Year:
                    if (Zoom < 50)
                    {
                        ZoomUnit = TimelineUnits.Decade;
                        Zoom = 500;
                        Offset = Offset / 10;
                    }
                    else if (Zoom > 600)
                    {
                        ZoomUnit = TimelineUnits.Month;
                        Zoom = 50;
                        Offset = Offset * 12;
                    }

                    break;

                case TimelineUnits.Decade:
                    break;

                case TimelineUnits.KYear:
                    break;

                case TimelineUnits.MYear:
                    break;
            }
        }

        //Detect orientation
        private void CheckOrientation()
        {
            orientation = TimelineOrientation.Portrait;
            if (this.Bounds.Width > this.Bounds.Height) orientation = TimelineOrientation.Landscape;
        }

        //Any touch action we simply forward to the gesture recognizer
        protected void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            //Console.WriteLine("Type:" + args.Type.ToString() + "  - ID:" + args.Id.ToString() + "  - Location: " + args.Location.ToString());
            gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, args.Location.ToSKPoint());
        }
    }
}
