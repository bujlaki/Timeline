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
            typeof(float),
            typeof(TimelineControl),
            (float)0, BindingMode.OneWay,
            propertyChanged: OnOffsetChanged);

        public static readonly BindableProperty ZoomProperty = BindableProperty.Create(
            nameof(Zoom),
            typeof(float),
            typeof(TimelineControl),
            (float)200, BindingMode.OneWay,
            propertyChanged: OnZoomChanged);

        public static readonly BindableProperty ZoomUnitProperty = BindableProperty.Create(
            nameof(ZoomUnit),
            typeof(TimelineUnits),
            typeof(TimelineControl),
            TimelineUnits.Year, BindingMode.OneWay,
            propertyChanged: OnZoomUnitChanged);
      
        public static readonly BindableProperty DateProperty = BindableProperty.Create(
            nameof(Date),
            typeof(TimelineControlDate),
            typeof(TimelineControl),
            new TimelineControlDate(), BindingMode.TwoWay,
            propertyChanged: OnZoomUnitChanged);
        
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

        private static void OnDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

        public float Offset
        {
            get { return (float)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public float Zoom
        {
            get { return (float)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public TimelineUnits ZoomUnit
        {
            get { return (TimelineUnits)GetValue(ZoomUnitProperty); }
            set { SetValue(ZoomUnitProperty, value); }
        }

        public TimelineControlDate Date
        {
            get { return (TimelineControlDate)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }

#endregion

        private string[] monthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private string[] shortMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        private TouchGestureRecognizer gestureRecognizer;
        private TimelineOrientation orientation;
        private TimelineControlTheme theme;
        private int timelineLeftPos;

        //private TimelineControlDate date;
        private TimelineControlDate unitDate;
        private TimelineControlDate subUnitDate;

        int unitMarkX1;
        int unitMarkX2;
        int subUnitMarkX1;
        int subUnitMarkX2;
        int unitTextX;
        int subUnitTextX;
        int halfHeight;

        public TimelineControl()
        {
            InitializeComponent();
            gestureRecognizer = new TouchGestureRecognizer();
            gestureRecognizer.OnGestureRecognized += GestureRecognizer_OnGestureRecognized;

            theme = new TimelineControlTheme();

            //date = new TimelineControlDate();
            unitDate = new TimelineControlDate();
            subUnitDate = new TimelineControlDate();

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
                    Offset -= (long)args.Data.Y * 2;
                    if(Offset>Zoom){
                        Offset -= Zoom;
                        Date.Add(ZoomUnit);
                    }
                    if(Offset<0){
                        Offset += Zoom;
                        Date.Add(ZoomUnit, -1);
                    }
                    canvasView.InvalidateSurface();
                    break;

                case TouchGestureType.Pinch:
                    Console.WriteLine("PINCH - " + args.Data.ToString());
                    float oldZoom = Zoom;
                    Zoom += (long)(args.Data.Y * (Zoom * 0.005));
                    Offset = Offset * Zoom / oldZoom;
                    AdjustZoomUnit();
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

            canvas.DrawLine(0, info.Height/2, info.Width, info.Height/2, theme.UnitMarkPaint);

            //DRAW UNITS
            unitMarkX1 = timelineLeftPos + theme.UnitMarkOffset;
            unitMarkX2 = unitMarkX1 + theme.UnitMarkLength;
            subUnitMarkX1 = timelineLeftPos + theme.SubUnitMarkOffset;
            subUnitMarkX2 = subUnitMarkX1 + theme.SubUnitMarkLength;
            unitTextX = timelineLeftPos + theme.UnitTextOffset;
            subUnitTextX = timelineLeftPos + theme.SubUnitTextOffset;
            halfHeight = info.Height / 2;

            DrawUnitsAndSubUnits(info, canvas);
        }

        private void DrawUnitsAndSubUnits(SKImageInfo info, SKCanvas canvas)
        {
            
            Date.CopyByUnit(ref unitDate, ZoomUnit);
            float unitPos = halfHeight - Offset;

            while (unitPos > 0)
            {
                unitPos -= Zoom;
                unitDate.Add(ZoomUnit, -1);
            }

            while (unitPos < info.Height)
            {
                canvas.DrawLine(unitMarkX1, unitPos, unitMarkX2, unitPos, theme.UnitMarkPaint);
                canvas.DrawText(GetUnitText(unitDate), unitTextX, unitPos + theme.UnitTextPaint.FontMetrics.CapHeight/2, theme.UnitTextPaint);

                unitDate.CopyByUnit(ref subUnitDate, ZoomUnit-1);
                float subUnitPos = unitPos;
                float subUnitStep = GetSubUnitStep(unitDate);
                bool showSubUnitText = subUnitStep > 30;
                while (subUnitDate.Value(ZoomUnit) == unitDate.Value(ZoomUnit) && subUnitPos < info.Height)
                {
                    canvas.DrawLine(subUnitMarkX1, subUnitPos, subUnitMarkX2, subUnitPos, theme.UnitMarkPaint);

                    if (showSubUnitText)
                        canvas.DrawText(GetSubUnitText(subUnitDate), subUnitTextX, subUnitPos + theme.UnitTextPaint.TextSize, theme.UnitTextPaint);
                    
                    subUnitPos += subUnitStep;
                    subUnitDate.Add(ZoomUnit-1);
                }

                unitDate.Add(ZoomUnit);
                unitPos += Zoom;
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
                case TimelineUnits.KYear:
                    return "";
                case TimelineUnits.MYear:
                    return "";
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
                    return tlcdate.baseDate.Year.ToString();
                case TimelineUnits.Century:
                    return tlcdate.Decade.ToString() + "0";
                case TimelineUnits.KYear:
                    return "";
                case TimelineUnits.MYear:
                    return "";
                default:
                    return "";
            }
        }

        private float GetSubUnitStep(TimelineControlDate tlcdate)
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    return 100;
                case TimelineUnits.Hour:
                    return (float)Zoom / 60;
                case TimelineUnits.Day:
                    return (float)Zoom / 24;
                case TimelineUnits.Month:
                    return (float)Zoom / DateTime.DaysInMonth(tlcdate.baseDate.Year, tlcdate.baseDate.Month);
                case TimelineUnits.Year:
                    return (float)Zoom / 12;
                case TimelineUnits.Decade:
                    return (float)Zoom / 10;
                case TimelineUnits.KYear:
                    return 100;
                case TimelineUnits.MYear:
                    return 100;
                default:
                    return 100;
            }
        }

        private void AdjustZoomUnit()
        {
            switch (this.ZoomUnit)
            {
                case TimelineUnits.Minute:
                    break;

                case TimelineUnits.Hour:
                    if (Zoom < 200)
                    {
                        ZoomUnit = TimelineUnits.Day;
                        Offset = Offset + Zoom * Date.baseDate.Hour;
                        Zoom = 4800;
                    }
                    break;

                case TimelineUnits.Day:
                    if (Zoom < 200)
                    {
                        ZoomUnit = TimelineUnits.Month;
                        Offset = Offset + Zoom * (Date.baseDate.Day - 1);
                        //Zoom = 175 * DateTime.DaysInMonth(date.baseDate.Year, date.baseDate.Month);
                        Zoom = 6000;
                        //Offset = Offset / DateTime.DaysInMonth(date.baseDate.Year, date.baseDate.Month);
                    }
                    else if (Zoom > 4800)
                    {
                        ZoomUnit = TimelineUnits.Hour;
                        Zoom = 200;
                        Offset = Offset - Zoom * Date.baseDate.Hour;
                    }
                    break;

                case TimelineUnits.Month:
                    if(Zoom<250)
                    {
                        ZoomUnit = TimelineUnits.Year;
                        Offset = Offset + Zoom * (Date.baseDate.Month - 1);
                        Zoom = 3000;
                    }
                    else if (Zoom>6000)
                    {
                        ZoomUnit = TimelineUnits.Day;
                        //Zoom = 6000 / DateTime.DaysInMonth(date.baseDate.Year, date.baseDate.Month);
                        Zoom = 200;
                        Offset = Offset - Zoom * (Date.baseDate.Day - 1);
                    }
                    break;

                case TimelineUnits.Year:
                    if (Zoom < 100)
                    {
                        ZoomUnit = TimelineUnits.Decade;
                        Offset = Offset + Zoom * Date.YearsInDecade();
                        Zoom = 1000;
                    }
                    else if (Zoom > 3000)
                    {
                        ZoomUnit = TimelineUnits.Month;
                        Zoom = 250;
                        Offset = Offset - Zoom * (Date.baseDate.Month -1);
                    }
                    break;

                case TimelineUnits.Decade:
                    if(Zoom>1000)
                    {
                        ZoomUnit = TimelineUnits.Year;
                        Zoom = 100;
                        Offset = Offset - Zoom * Date.YearsInDecade();
                    }
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
            gestureRecognizer.ProcessTouchEvent(args.Id, args.Type, args.Location.ToSKPoint());
        }
    }
}
