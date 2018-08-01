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
        ItemList
    }

	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ValuePicker : ContentView
	{
        #region "Bindable properties"
        public static readonly BindableProperty PickerTypeProperty = BindableProperty.Create(
            nameof(PickerType),
            typeof(ValuePickerType),
            typeof(ValuePicker),
            ValuePickerType.Numeric, BindingMode.OneWay,
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

        public static readonly BindableProperty ItemsProperty = BindableProperty.Create(
            nameof(Items),
            typeof(IEnumerable<object>),
            typeof(ValuePicker),
            null, BindingMode.TwoWay,
            propertyChanged: OnItemsChanged);

        private static void OnItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ValuePicker vp = (ValuePicker)bindable;
            if (newValue!=null)
            {
                vp.MinValue = 0;
                vp.MaxValue = vp.Items.Count() - 1;
            }
            
            vp.doInitialize = true;
            vp.canvasView.InvalidateSurface();
        }
        public IEnumerable<object> Items
        {
            get { return (IEnumerable<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly BindableProperty MinValueProperty = BindableProperty.Create(
            nameof(MinValue),
            typeof(int),
            typeof(ValuePicker),
            0, BindingMode.OneWay,
            propertyChanged: OnMinValueChanged);

        private static void OnMinValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ValuePicker vp = (ValuePicker)bindable;
            if (vp.NumericValue < vp.MinValue) vp.NumericValue = vp.MinValue;
            if (vp.NumericValue > vp.MaxValue) vp.NumericValue = vp.MaxValue;
            vp.doInitialize = true;
            vp.canvasView.InvalidateSurface();
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
            100, BindingMode.OneWay,
            propertyChanged: OnMaxValueChanged);

        private static void OnMaxValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ValuePicker vp = (ValuePicker)bindable;
            if (vp.NumericValue < vp.MinValue) vp.NumericValue = vp.MinValue;
            if (vp.NumericValue > vp.MaxValue) vp.NumericValue = vp.MaxValue;
            vp.doInitialize = true;
            vp.canvasView.InvalidateSurface();
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
            ValuePicker vp = (ValuePicker)bindable;
            if (!vp.inAction) vp.currentOffset = (vp.NumericValue - vp.MinValue) * vp.stepY;
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => vp.canvasView.InvalidateSurface());
        }
        public int NumericValue
        {
            get { return (int)GetValue(NumericValueProperty); }
            set { SetValue(NumericValueProperty, value); }
        }

        public static readonly BindableProperty ItemValueProperty = BindableProperty.Create(
            nameof(ItemValue),
            typeof(object),
            typeof(ValuePicker),
            null, BindingMode.TwoWay,
            propertyChanged: OnItemValueChanged);

        private static void OnItemValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ValuePicker vp = (ValuePicker)bindable;
            if (!vp.inAction) vp.NumericValue = vp.Items.ToList().IndexOf(newValue);
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => vp.canvasView.InvalidateSurface());
        }
        public object ItemValue
        {
            get { return (object)GetValue(ItemValueProperty); }
            set { SetValue(ItemValueProperty, value); }
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
        private int maxFontSize;
        private int dFontSize;

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
        private Timer adjustTimer;
        private float adjustToOffset;

        private bool inAction;

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
            swipeTimer = new Timer(SwipeAnimationCallback, null, Timeout.Infinite, Timeout.Infinite);
            adjustTimer = new Timer(AdjustAnimationCallback, null, Timeout.Infinite, Timeout.Infinite);
            doInitialize = true;
            inAction = false;
        }

        private void SwipeAnimationCallback(object state)
        {
            currentOffset += swipeSpeed;

            if (currentOffset > maxValueOffset) { currentOffset = maxValueOffset; StopSwipeAnimation(); }
            else if (currentOffset <minValueOffset) { currentOffset = minValueOffset; StopSwipeAnimation(); }
            else
            {
                SetValue();
                swipeSpeed = swipeSpeed * 0.9f;
                if (Math.Abs(swipeSpeed) < stepY / 10) { StopSwipeAnimation(); StartAdjustAnimation(100); }
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
            }
        }

        private void AdjustAnimationCallback(object state)
        {
            float diff = adjustToOffset - currentOffset;

            if(Math.Abs(diff) < stepY / 20)
            {
                currentOffset = adjustToOffset;
                StopAdjustAnimation();
            }
            else
            {
                currentOffset = currentOffset + diff * 0.3f;
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
            }
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

            int minVisibleValue = MinValue + (int)((currentOffset - halfHeight) / stepY);
            int maxVisibleValue = MinValue + (int)((currentOffset + halfHeight) / stepY) + 1;

            int unit;
            for(unit=minVisibleValue; unit<=maxVisibleValue; unit++)
            {
                if (unit < MinValue || unit > MaxValue) continue;
                float unitOffset = minValueOffset + (unit - MinValue) * stepY;

                float ydiff = Math.Abs(currentOffset - unitOffset);
                //int fontSize = maxFontSize - (int)(Math.Sqrt(Math.Abs(ydiff / halfHeight)) * dFontSize);
                //int fontSize = maxFontSize - (int)(Math.Pow(ydiff / halfHeight,2) * dFontSize);
                int fontSize = maxFontSize - (int)(ydiff / halfHeight * dFontSize);
                primaryPaint.TextSize = fontSize;

                if (PickerType == ValuePickerType.Numeric)
                {
                    canvas.DrawTextOnPath(unit.ToString(), path, 0, unitOffset - currentOffset + primaryPaint.FontMetrics.CapHeight / 2, primaryPaint);
                }
                else if(PickerType==ValuePickerType.ItemList)
                {
                    if(Items!=null)
                        canvas.DrawTextOnPath(Items.ElementAt(unit).ToString(), path, 0, unitOffset - currentOffset + primaryPaint.FontMetrics.CapHeight / 2, primaryPaint);
                }
            }
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch(e.StatusType)
            {
                case GestureStatus.Started:
                    StopSwipeAnimation();
                    StopAdjustAnimation();
                    panStart = DateTime.UtcNow;
                    panStartOffset = currentOffset;
                    inAction = true;
                    break;
                case GestureStatus.Running:
                    currentOffset = panStartOffset - (float)e.TotalY * 2;
                    if (currentOffset < minValueOffset) currentOffset = minValueOffset;
                    if (currentOffset > maxValueOffset) currentOffset = maxValueOffset;
                    SetValue();
                    break;
                case GestureStatus.Completed:
                    inAction = false;
                    TimeSpan panTime = DateTime.UtcNow - panStart;
                    float totalPan = currentOffset - panStartOffset;

                    if (panTime < swipeTimeMax)
                        StartSwipeAnimation((100 / panTime.Milliseconds) * totalPan, 100);
                    else
                        StartAdjustAnimation(100);
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
            stepY = fullHeight / 6;
            halfStepY = stepY / 2.0f;
            maxFontSize = (int)(stepY);
            dFontSize = (int)(maxFontSize * 0.6f);

            minValueOffset = 0.0f;
            maxValueOffset = (MaxValue - MinValue) * stepY;
            currentOffset = (NumericValue - MinValue) * stepY;

            var x = Xamarin.Essentials.DeviceDisplay.ScreenMetrics.Density;
            var xx = Xamarin.Essentials.DeviceDisplay.ScreenMetrics.Height;
            var xxx = Xamarin.Essentials.DeviceDisplay.ScreenMetrics.Width;

            highlighterPaint.StrokeWidth = stepY;
        }

        private void StartSwipeAnimation(float speed, int msInterval)
        {
            inAction = true;
            swipeSpeed = speed;
            swipeTimer.Change(msInterval, msInterval);
        }

        private void StopSwipeAnimation()
        {
            swipeSpeed = 0.0f;
            swipeTimer.Change(Timeout.Infinite, Timeout.Infinite);
            SetValue();
            inAction = false;
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        }

        private void StartAdjustAnimation(int msInterval)
        {
            inAction = true;
            adjustToOffset = (NumericValue - MinValue) * stepY;
            adjustTimer.Change(msInterval, msInterval);
        }

        private void StopAdjustAnimation()
        {
            adjustTimer.Change(Timeout.Infinite, Timeout.Infinite);
            inAction = false;
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => canvasView.InvalidateSurface());
        }

        private void SetValue()
        {
            NumericValue = MinValue + (int)((currentOffset + halfStepY) / stepY);
            if (PickerType == ValuePickerType.ItemList) ItemValue = Items.ElementAt(NumericValue);
        }
    }
}