using System;
using System.Collections.Generic;

using Xamarin.Forms;
using SkiaSharp;

namespace TouchTracking
{
    public enum TouchGestureType
    {
        Drag,
        Zoom
    }

    public class TouchGestureEventArgs : EventArgs
    {
        public TouchGestureEventArgs(long id, TouchGestureType type, Point data)
        {
            Id = id;
            Type = type;
            Data = data;
        }

        public long Id { private set; get; }

        public TouchGestureType Type { private set; get; }

        public Point Data { private set; get; }
    }

    public delegate void TouchGestureEventHandler(object sender, TouchGestureEventArgs args);

    public class TouchGestureRecognizer
    {
        Dictionary<long, TouchInfo> touches = new Dictionary<long, TouchInfo>();

        public event TouchGestureEventHandler OnGestureRecognized;

        public TouchGestureRecognizer()
        {
        }

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    if (touches.Count < 2)
                    {
                        touches.Add(id, new TouchInfo
                        {
                            PreviousPoint = location,
                            NewPoint = location
                        });
                    }
                    break;

                case TouchActionType.Moved:
                    TouchInfo info = touches[id];
                    if (info == null) break;
                    info.NewPoint = location;
                    if (touches.Count == 1)
                    {
                        Point data = new Point(location.X - info.PreviousPoint.X, location.Y - info.PreviousPoint.Y);
                        OnGestureRecognized(this, new TouchGestureEventArgs(id, TouchGestureType.Drag, data));
                    } else if(touches.Count==2){
                        
                    }
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touches[id].NewPoint = location;
                    //Manipulate();
                    touches.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touches.Remove(id);
                    break;
            }
        }
    }
}
