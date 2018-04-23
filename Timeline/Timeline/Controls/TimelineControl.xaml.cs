using System;
using System.Collections.Generic;
using System.Globalization;
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
        MYear
    }

    public enum TimelineOrientation {
        Portrait,
        Landscape
    }

    public partial class TimelineControl : ContentView
    {
        private string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        private TimelineOrientation orientation;

        public static readonly BindableProperty OffsetProperty = BindableProperty.Create(
            nameof(Offset),
            typeof(long),
            typeof(TimelineControl),
            (long)-DateTime.Now.Year * 100, BindingMode.OneWay,
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

        private TouchGestureRecognizer gestureRecognizer;

        public TimelineControl()
        {
            InitializeComponent();
            gestureRecognizer = new TouchGestureRecognizer();
            gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

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

            //draw the blue line
            SKPaint paintLine = new SKPaint();
            paintLine.Color = Color.SkyBlue.ToSKColor();
            if (orientation == TimelineOrientation.Portrait)
            {
                paintLine.StrokeWidth = 100;
                canvas.DrawLine(info.Width - 50, 0, info.Width - 50, info.Height, paintLine);
            } else {
                paintLine.StrokeWidth = 50;
                canvas.DrawLine(0, 25, info.Width, 25, paintLine);
            }



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

        private void DrawMonths(SKImageInfo info, SKCanvas canvas)
        {
            int firstVisibleMonth = (int)(-this.Offset / this.Zoom + 1);
            int lastVisibleMonth = (int)((-this.Offset + info.Height) / this.Zoom + 1);

            SKPath path = new SKPath();
            path.MoveTo(info.Width - 45, 0);
            path.LineTo(info.Width - 45, info.Height);

            SKPaint paintText = new SKPaint();
            paintText.Color = Color.Black.ToSKColor();
            paintText.TextSize = 32;

            long monthPos = firstVisibleMonth * this.Zoom + this.Offset;
            for (int i = firstVisibleMonth; i < lastVisibleMonth; i++)
            {
                int year = i / 12;
                int month = i % 12;
                if(month==0)
                    canvas.DrawTextOnPath(year.ToString(), path, monthPos, 0, paintText);
                else
                    canvas.DrawTextOnPath(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month), path, monthPos, 0, paintText);
                monthPos += this.Zoom;
            }
        }

        private void DrawYears(SKImageInfo info, SKCanvas canvas)
        {
            int firstVisibleYear = (int)(-this.Offset / this.Zoom + 1);
            int lastVisibleYear = (int)((-this.Offset + info.Height) / this.Zoom + 1);

            SKPath path = new SKPath();
            path.MoveTo(info.Width - 45, 0);
            path.LineTo(info.Width - 45, info.Height);

            SKPaint paintText = new SKPaint();
            paintText.Color = Color.Black.ToSKColor();
            paintText.TextSize = 32;

            long yearPos = firstVisibleYear * this.Zoom + this.Offset;
            int xpos = info.Width - 100;
            for (int i = firstVisibleYear; i < lastVisibleYear; i++)
            {
                canvas.DrawText(i.ToString(), new SKPoint(xpos, yearPos), paintText);
                //canvas.DrawTextOnPath(i.ToString(), path, yearPos, 0, paintText);
                yearPos += this.Zoom;
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
