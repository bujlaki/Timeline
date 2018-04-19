using System;
using System.Collections.Generic;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    public partial class TimelineControl : ContentView
    {
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
        
        private static void OnStartYearChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
        }

        private static void OnEndYearChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((TimelineControl)bindable).InvalidateLayout();
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
            paintLine.StrokeWidth = 25;
            canvas.DrawLine(info.Width-30, 0, info.Width-30, info.Height, paintLine);

            SKRect rect = new SKRect(info.Width / 10, info.Height / 10, info.Width / 10 * 9, info.Height / 10 * 9);
            SKSize size = new SKSize(5, 5);
            SKPaint rectPaint = new SKPaint();
            rectPaint.Color = new SKColor(128, 128, 128);
            canvas.DrawRoundRect(rect, size, rectPaint);

        }
    }
}
