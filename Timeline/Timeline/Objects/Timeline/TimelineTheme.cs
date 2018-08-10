using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using SkiaSharp;

using Xamarin.Essentials;

namespace Timeline.Objects.Timeline
{
    public class TimelineTheme
    {
        public SKPaint TimelinePaint { get; set; }
        public SKPaint UnitMarkPaint { get; set; }
        public SKPaint UnitTextPaint { get; set; }
        public SKPaint SubUnitMarkPaint { get; set; }
        public SKPaint SubUnitTextPaint { get; set; }
        public SKPaint HighlightPaint { get; set; }
        public SKPaint EventPaint { get; set; }
        public SKPaint EventBorderPaint { get; set; }
        public SKPaint EventTextPaint { get; set; }

        public TimelineTheme(string userid)
        {
            Load("");
        }

        public void Load(string userid)
        {
            TimelinePaint = new SKPaint();
            TimelinePaint.Color = SKColor.Parse(Preferences.Get("timeline_color", "#bfd9f3"));

            UnitMarkPaint = new SKPaint();
            UnitMarkPaint.Color = SKColor.Parse(Preferences.Get("unitmark_color", SKColors.Black.ToString()));
            UnitMarkPaint.StrokeWidth = 4;

            UnitTextPaint = new SKPaint();
            UnitTextPaint.Color = SKColor.Parse(Preferences.Get("unittext_color", SKColors.Black.ToString()));

            SubUnitMarkPaint = new SKPaint();
            SubUnitMarkPaint.Color = SKColor.Parse(Preferences.Get("subunitmark_color", SKColors.DimGray.ToString()));

            SubUnitTextPaint = new SKPaint();
            SubUnitTextPaint.Color = SKColor.Parse(Preferences.Get("subunittext_color", SKColors.DimGray.ToString()));

            HighlightPaint = new SKPaint();
            HighlightPaint.Color = SKColor.Parse(Preferences.Get("highlight_color", SKColors.Yellow.ToString()));

            EventPaint = new SKPaint();
            EventPaint.Color = SKColor.Parse(Preferences.Get("event_color", SKColors.DarkGray.ToString()));
            EventPaint.Style = SKPaintStyle.Fill;

            EventBorderPaint = new SKPaint();
            EventBorderPaint.Color = SKColor.Parse(Preferences.Get("eventborder_color", SKColors.Black.ToString()));
            EventBorderPaint.StrokeWidth = 4;
            EventBorderPaint.Style = SKPaintStyle.Stroke;

            EventTextPaint = new SKPaint();
            EventTextPaint.Color = SKColor.Parse(Preferences.Get("eventtext_color", SKColors.White.ToString()));
            EventTextPaint.TextSize = 32;
        }
           
        public void Save(string userid)
        {
            //TODO: IMPLEMENT STORED PREFERENCES PER USER
            Preferences.Set("timeline_color", TimelinePaint.Color.ToString());
            Preferences.Set("unitmark_color", UnitMarkPaint.Color.ToString());
            Preferences.Set("unittext_color", UnitTextPaint.Color.ToString());
            Preferences.Set("subunitmark_color", SubUnitMarkPaint.Color.ToString());
            Preferences.Set("subunittext_color", SubUnitTextPaint.Color.ToString());
            Preferences.Set("highlight_color", HighlightPaint.Color.ToString());
            Preferences.Set("event_color", EventPaint.Color.ToString());
            Preferences.Set("eventborder_color", EventBorderPaint.Color.ToString());
            Preferences.Set("eventtext_color", EventTextPaint.Color.ToString());
        }
    }
}
