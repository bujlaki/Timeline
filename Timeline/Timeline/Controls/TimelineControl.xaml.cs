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
        
#region "Bindable properties"
        public static readonly BindableProperty OffsetProperty = BindableProperty.Create(
            nameof(Offset),
            typeof(long),
            typeof(TimelineControl),
            (long)-10 * 100, BindingMode.OneWay,
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

        private SKColor summaryTextColor;
        private SKColor unitMarkColor;
        private SKColor unitColor1;
        private SKColor unitColor2;
        private SKColor subUnitColor1;
        private SKColor subUnitColor2;

        private SKPaint summaryTextPaint;
        private SKPaint unitMarkPaint;
        private SKPaint unitPaint;
        private SKPaint subUnitPaint;
        private SKPaint unitTextPaint;
        private SKPaint subUnitTextPaint;

        private SKPath summaryPath;
        private SKPath unitPath;
        private SKPath subUnitPath;

        public TimelineControl()
        {
            InitializeComponent();
            gestureRecognizer = new TouchGestureRecognizer();
            gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

            //init gui
            summaryTextColor = Color.Blue.ToSKColor();
            unitMarkColor = Color.Black.ToSKColor();
            unitColor1 = Color.AliceBlue.ToSKColor();
            unitColor2 = Color.AntiqueWhite.ToSKColor();
            subUnitColor1 = Color.Azure.ToSKColor();
            subUnitColor2 = Color.Blue.ToSKColor();

            summaryTextPaint = new SKPaint();
            summaryTextPaint.Color = summaryTextColor;
            summaryTextPaint.TextSize = 24;

            unitMarkPaint = new SKPaint();
            unitMarkPaint.Color = unitMarkColor;
            unitMarkPaint.StrokeWidth = 2;

            unitPaint = new SKPaint();
            unitPaint.Color = unitColor1;
            unitPaint.StrokeWidth = 25;

            subUnitPaint = new SKPaint();
            subUnitPaint.Color = subUnitColor1;
            subUnitPaint.StrokeWidth = 25;

            unitTextPaint = new SKPaint();
            unitTextPaint.Color = Color.Black.ToSKColor();
            unitTextPaint.TextSize = 24;

            subUnitTextPaint = new SKPaint();
            subUnitTextPaint.Color = Color.Black.ToSKColor();
            subUnitTextPaint.TextSize = 24;

            summaryPath = new SKPath();
            unitPath = new SKPath();
            subUnitPath = new SKPath();

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

            summaryPath.Reset();
            summaryPath.MoveTo(info.Width - 20, 0);
            summaryPath.LineTo(info.Width - 20, info.Height);

            unitPath.Reset();
            unitPath.MoveTo(info.Width - 60, 0);
            unitPath.LineTo(info.Width - 60, info.Height);

            subUnitPath.Reset();
            subUnitPath.MoveTo(info.Width - 100, 0);
            subUnitPath.LineTo(info.Width - 100, info.Height);


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
            long firstVisibleMonth = -this.Offset / this.Zoom;
            long lastVisibleMonth = (-this.Offset + info.Height) / this.Zoom + 1;

            //summary text - century
            string summaryText = "1st century";
            int summaryTextWidth = (int)summaryTextPaint.MeasureText(summaryText);
            canvas.DrawTextOnPath(summaryText, summaryPath, (info.Height - summaryTextWidth) / 2, 0, summaryTextPaint);

            //main unit data - MONTH
            //optional subunit data - DAY
            long monthPos = firstVisibleMonth * this.Zoom + this.Offset;
            for (long i = firstVisibleMonth; i < lastVisibleMonth; i++)
            {
                int year = (int)(i / 12);
                int month = (int)(i % 12 + 1);
                canvas.DrawLine(info.Width - 100, monthPos, info.Width - 40, monthPos, unitMarkPaint);
                canvas.DrawTextOnPath(shortMonthNames[month-1], unitPath, monthPos, 0, unitTextPaint);
                //canvas.DrawTextOnPath(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month), unitPath, monthPos, 0, unitTextPaint);

                //subunit data
                if(Zoom>500)
                {
                    int days = DateTime.DaysInMonth(year, month);
                    float dayStep = (float)Zoom / days;
                    for (int day = 0; day < days; day++)
                    {
                        float dayPos = monthPos + day * dayStep;
                        canvas.DrawLine(info.Width - 100, dayPos, info.Width - 60, dayPos, unitMarkPaint);
                    }
                }

                monthPos += this.Zoom;
            }
        }

        private void DrawYears(SKImageInfo info, SKCanvas canvas)
        {
            int firstVisibleYear = (int)(-this.Offset / this.Zoom);
            int lastVisibleYear = (int)((-this.Offset + info.Height) / this.Zoom + 1);

            //summary text - century
            string summaryText = "1st century";
            int summaryTextWidth = (int)summaryTextPaint.MeasureText(summaryText);
            canvas.DrawTextOnPath(summaryText, summaryPath, (info.Height - summaryTextWidth) / 2, 0, summaryTextPaint);

            //main unit data - YEAR
            //optional subunit data - MONTH
            long yearPos = firstVisibleYear * this.Zoom + this.Offset;
            int xpos = info.Width - 100;
            int unitXpos = info.Width - 75;
            for (int i = firstVisibleYear; i < lastVisibleYear; i++)
            {
                canvas.DrawLine(info.Width - 100, yearPos, info.Width - 40, yearPos, unitMarkPaint);
                canvas.DrawTextOnPath(i.ToString(), unitPath, yearPos, 0, unitTextPaint);

                //subunit data
                if (Zoom > 200)
                {
                    long monthStep = Zoom / 12;
                    for (int month = 0; month < 12; month++)
                    {
                        long monthPos = yearPos + month * monthStep;
                        canvas.DrawLine(info.Width - 100, monthPos, info.Width - 60, monthPos, unitMarkPaint);
                    }
                }

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
