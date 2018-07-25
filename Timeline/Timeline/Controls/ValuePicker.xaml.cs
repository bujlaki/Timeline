using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Threading;

namespace Timeline.Controls
{
    public enum ValuePickerType
    {
        Numeric,
        StringList
    }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ValuePicker : ContentView
	{
        #region "Bindable properties"
        public static readonly BindableProperty PickerTypeProperty = BindableProperty.Create(
            nameof(PickerType),
            typeof(ValuePickerType),
            typeof(ValuePicker),
            ValuePickerType.Numeric, BindingMode.TwoWay,
            propertyChanged: OnPickerTypeChanged);

        private static void OnPickerTypeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ValuePicker)bindable).InvalidateLayout();
        }
        public ValuePickerType PickerType
        {
            get { return (ValuePickerType)GetValue(PickerTypeProperty); }
            set { SetValue(PickerTypeProperty, value); }
        }

        public static readonly BindableProperty MinValueProperty = BindableProperty.Create(
            nameof(MinValue),
            typeof(int),
            typeof(ValuePicker),
            0, BindingMode.TwoWay,
            propertyChanged: OnMinValueChanged);

        private static void OnMinValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ValuePicker)bindable).InvalidateLayout();
        }
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
            nameof(MaxValue),
            typeof(int),
            typeof(ValuePicker),
            100, BindingMode.TwoWay,
            propertyChanged: OnMaxValueChanged);

        private static void OnMaxValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ValuePicker)bindable).InvalidateLayout();
        }
        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly BindableProperty NumericValueProperty = BindableProperty.Create(
            nameof(NumericValue),
            typeof(int),
            typeof(ValuePicker),
            10, BindingMode.TwoWay,
            propertyChanged: OnNumericValueChanged);

        private static void OnNumericValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((ValuePicker)bindable).InvalidateLayout();
        }
        public int NumericValue
        {
            get { return (int)GetValue(NumericValueProperty); }
            set { SetValue(NumericValueProperty, value); }
        }
        #endregion

        private SKPaint primaryPaint;
        private SKPaint highlighterPaint;

        private bool doInitialize;
        private int halfHeight;
        private int fullHeight;
        private int halfWidth;
        private int fullWidth;
        private int stepY;
        private float halfStepY;

        //VISUALIZE
        private float minValueOffset;
        private float maxValueOffset;
        private float currentOffset;

        //PAN - SWIPE gesture
        private DateTime panStart;
        private float panStartOffset;
        private float swipeSpeed;
        private TimeSpan swipeTimeMax;
        private Timer swipeTimer;

        public ValuePicker ()
		{
			InitializeComponent ();

            primaryPaint = new SKPaint();
            primaryPaint.TextSize = 32;
            primaryPaint.Color = Color.Black.ToSKColor();
            primaryPaint.TextAlign = SKTextAlign.Center;
            highlighterPaint = new SKPaint();
            highlighterPaint.Color = Color.Yellow.ToSKColor();

            swipeTimeMax = TimeSpan.FromMilliseconds(600);
            swipeTimer = new Timer(SwipeTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            doInitialize = true;
        }

        private void SwipeTimerCallback(object state)
        {
            currentOffset += swipeSpeed;
            
            swipeSpeed = swipeSpeed * 0.9f;

            if(Math.Abs(swipeSpeed) < stepY / 10)
            {
                swipeSpeed = 0.0f;
                swipeTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            if (doInitialize) Initialize(info);
            doInitialize = false;

            canvas.Clear();

            canvas.DrawLine(0, halfHeight, fullWidth, halfHeight, highlighterPaint);

            DrawValuesNumeric(canvas);
        }

        private void DrawValuesNumeric(SKCanvas canvas)
        {
            //draw current value
            SKPath path = new SKPath();
            path.MoveTo(0, halfHeight);
            path.LineTo(fullWidth, halfHeight);

            int minVisibleValue = (int)((currentOffset - halfHeight) / stepY);
            int maxVisibleValue = (int)((currentOffset + halfHeight) / stepY) + 1;

            int unit;
            for(unit=minVisibleValue; unit<=maxVisibleValue; unit++)
            {
                if (unit < MinValue || unit > MaxValue) continue;
                float unitOffset = minValueOffset + (unit - MinValue) * stepY;
                canvas.DrawTextOnPath(unit.ToString(), path, 0, unitOffset - currentOffset + primaryPaint.FontMetrics.CapHeight/2, primaryPaint);
            }
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch(e.StatusType)
            {
                case GestureStatus.Started:
                    swipeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    panStart = DateTime.UtcNow;
                    panStartOffset = currentOffset;
                    break;
                case GestureStatus.Running:
                    currentOffset = panStartOffset - (float)e.TotalY * 2;
                    if (currentOffset < minValueOffset) currentOffset = minValueOffset;
                    if (currentOffset > maxValueOffset) currentOffset = maxValueOffset;
                    NumericValue = (int)((currentOffset + halfStepY) / stepY);
                    break;
                case GestureStatus.Completed:
                    TimeSpan panTime = DateTime.UtcNow - panStart;
                    float totalPan = currentOffset - panStartOffset;
                    if(panTime < swipeTimeMax)
                    {
                        swipeSpeed = 100 / panTime.Milliseconds * totalPan;
                        swipeTimer.Change(100, 100);
                    }
                    //currentOffset = panStartOffset - (float)e.TotalY;
                    break;
            }

            canvasView.InvalidateSurface();
        }

        private void Initialize(SKImageInfo info)
        {
            halfHeight = info.Height / 2;
            fullHeight = info.Height;
            halfWidth = info.Width / 2;
            fullWidth = info.Width;
            stepY = info.Height / 6;
            halfStepY = (float)stepY / 2.0f;

            minValueOffset = 0.0f;
            maxValueOffset = (MaxValue - MinValue) * stepY;
            currentOffset = (NumericValue - MinValue) * stepY;

            highlighterPaint.StrokeWidth = stepY;
        }
    }
}