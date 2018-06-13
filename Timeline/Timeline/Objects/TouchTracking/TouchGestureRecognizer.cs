using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using SkiaSharp;

namespace Timeline.Objects.TouchTracking
{
    public class Timings
    {
        public static int shortTapMilliseconds = 200;
        public static int longTapMilliseconds = 1000;
    }

    public enum TouchGestureType
    {
        Tap,
        LongTap,
        Pan,
        Swipe,
        Pinch
    }

    public class TouchGestureEventArgs : EventArgs
    {
        public TouchGestureEventArgs(long id, TouchGestureType type, SKPoint data)
        {
            Id = id;
            Type = type;
            Data = data;
        }

        public long Id { private set; get; }

        public TouchGestureType Type { private set; get; }

        public SKPoint Data { private set; get; }
    }

    public delegate void TouchGestureEventHandler(object sender, TouchGestureEventArgs args);

    public class TouchGestureRecognizer
    {
        Dictionary<long, TouchInfo> touches = new Dictionary<long, TouchInfo>();

        public event TouchGestureEventHandler OnGestureRecognized;

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            TouchInfo info;

            switch (type)
            {
                case TouchActionType.Pressed:
                    //we only handle 2 fingers
                    if (touches.Count < 2)
                    {
                        touches.Add(id, new TouchInfo
                        {
                            InitialTime = DateTime.UtcNow,
                            InitialPoint = location,
                            PreviousPoint = location,
                            NewPoint = location
                        });

                        //check for LONGTAP
                        CheckLongTapAsync(Timings.longTapMilliseconds, id);
                    }
                    break;

                case TouchActionType.Moved:
                    info = touches[id];
                    if (info == null) break;

                    info.NewPoint = location;

                    if (touches.Count == 1)
                    {
                        //this is PAN
                        SKPoint diff = info.NewPoint - info.PreviousPoint;
                        OnGestureRecognized(this, new TouchGestureEventArgs(id, TouchGestureType.Pan, diff));
                    } else if(touches.Count==2){
                        //this is PINCH
                        TouchInfo info1 = touches.ElementAt(0).Value;
                        TouchInfo info2 = touches.ElementAt(1).Value;
                        SKPoint preDiff = new SKPoint(Math.Abs(info2.PreviousPoint.X - info1.PreviousPoint.X),Math.Abs(info2.PreviousPoint.Y-info1.PreviousPoint.Y));
                        SKPoint postDiff = new SKPoint(Math.Abs(info2.NewPoint.X - info1.NewPoint.X), Math.Abs(info2.NewPoint.Y - info1.NewPoint.Y));
                        SKPoint pinchData = postDiff - preDiff;
                        OnGestureRecognized(this, new TouchGestureEventArgs(id, TouchGestureType.Pinch, pinchData));
                    }
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:       
                    info = touches[id];
                    if (info == null) break;

                    info.NewPoint = location;

                    if(touches.Count==1){
                        SKPoint diff = info.NewPoint - info.InitialPoint;
                        if (Math.Abs(diff.X) < 5 && Math.Abs(diff.Y) < 5)
                        {
                            //check for TAP
                            TimeSpan timeDiff = DateTime.UtcNow - info.InitialTime;
                            if (timeDiff < TimeSpan.FromMilliseconds(Timings.shortTapMilliseconds))
                            {
                                OnGestureRecognized(this, new TouchGestureEventArgs(id, TouchGestureType.Tap, info.InitialPoint));
                            }
                        } else {
                            //check for SWIPE
                            SKPoint lastDiff = info.NewPoint - info.PreviousPoint;
                            if(Math.Abs(lastDiff.X)>5 || Math.Abs(lastDiff.Y)>5)
                            {
                                OnGestureRecognized(this, new TouchGestureEventArgs(id, TouchGestureType.Swipe, lastDiff));
                            }
                        }
                    }
                    touches.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touches.Remove(id);
                    break;
            }
        }
        
        public async Task CheckLongTapAsync(int delay, long id)
        {
            await Task.Delay(delay);

            if (touches.Count!=1) return;
            TouchInfo info = touches[id];
            if (info == null) return;

            float diffX = Math.Abs(info.PreviousPoint.X - info.InitialPoint.X);
            float diffY = Math.Abs(info.PreviousPoint.Y - info.InitialPoint.Y);
            if (diffX < 5 && diffY < 5)
                OnGestureRecognized(this, new TouchGestureEventArgs(id, TouchGestureType.LongTap, info.InitialPoint));
        }


    }
}
