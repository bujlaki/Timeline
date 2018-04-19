using System;
using System.Collections.Generic;
using Xamarin.Forms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace Timeline.Controls
{
    public partial class TimelineEventControl : ContentView
    {
        public TimelineEventControl()
        {
            InitializeComponent();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();
            SKRect rect = new SKRect(info.Width / 10, info.Height / 10, info.Width / 10 * 9, info.Height / 10 * 9);
            SKSize size = new SKSize(5, 5);
            SKPaint rectPaint = new SKPaint();
            rectPaint.Color = new SKColor(128, 128, 128);
            canvas.DrawRoundRect(rect, size, rectPaint);

        }
    }
}
