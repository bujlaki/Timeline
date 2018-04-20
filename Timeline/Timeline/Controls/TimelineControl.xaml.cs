﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    public enum TimelineUnits {
        Day,
        Month,
        Year,
        KYear,
        MYear
    }

    public partial class TimelineControl : ContentView
    {
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
            set { SetValue(ZoomProperty, value); }
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

        public TimelineControl()
        {
            InitializeComponent();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPaint paintLine = new SKPaint();
            paintLine.Color = Color.SkyBlue.ToSKColor();
            paintLine.StrokeWidth = 50;
            canvas.DrawLine(info.Width-25, 0, info.Width-25, info.Height, paintLine);



            switch(this.ZoomUnit)
            {
                case TimelineUnits.Day:
                    break;

                case TimelineUnits.Month:
                    break;

                case TimelineUnits.Year:
                    DrawYears(info, canvas);
                    break;

                case TimelineUnits.KYear:
                    break;

                case TimelineUnits.MYear:
                    break;
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
            for (int i = firstVisibleYear; i < lastVisibleYear; i++)
            {
                canvas.DrawTextOnPath(i.ToString(), path, yearPos, 0, paintText);
                yearPos += this.Zoom;
            }

        }

        void Handle_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            Console.WriteLine("Type:" + e.ActionType.ToString() + "  - ID:" + e.Id.ToString());
            e.Handled = true;
            //switch(e.ActionType)
            //{
            //    case SKTouchAction.Pressed:
            //        Console.WriteLine("Pressed: " + e.Id.ToString());
            //        break;
            //    case SKTouchAction.Cancelled:
            //        Console.WriteLine("Cancelled: " + e.Id.ToString());
            //        break;
            //    case SKTouchAction.Entered:
            //        Console.WriteLine("Entered: " + e.Id.ToString());
            //        break;
            //    case SKTouchAction.Exited:
            //        Console.WriteLine("Exited: " + e.Id.ToString());
            //        break;
            //    case SKTouchAction.Moved:
            //        Console.WriteLine("Moved: " + e.Id.ToString());
            //        break;
            //    case SKTouchAction.Released:
            //        Console.WriteLine("Released: " + e.Id.ToString());
            //        break;

            //}
            //if (e.ActionType == SKTouchAction.Pressed)
            //{
            //    this.Offset += 10;
            //    canvasView.InvalidateSurface();
            //}
            //throw new NotImplementedException();
        }
    }
}
