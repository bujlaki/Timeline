using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xamarin.Forms;

using SkiaSharp;
using skia = SkiaSharp.Views.Forms;

using Xamarin.Essentials;

namespace Timeline.Objects.Timeline
{
    public class TimelineTheme
    {
        public Xamarin.Forms.Color bkgColor1;
        public Xamarin.Forms.Color bkgColor2;
        public Xamarin.Forms.Color bkgColor3;
        public Xamarin.Forms.Color textColor1;
        public Xamarin.Forms.Color textColor2;

        public SKPaint TimelinePaint { get; set; }
        public SKPaint UnitMarkPaint { get; set; }
        public SKPaint UnitTextPaint { get; set; }
        public SKPaint SubUnitMarkPaint { get; set; }
        public SKPaint SubUnitTextPaint { get; set; }
        public SKPaint HighlightPaint { get; set; }
        public SKPaint EventPaint { get; set; }
        public SKPaint EventBorderPaint { get; set; }
        public SKPaint EventTextPaint { get; set; }
        public SKPaint SummaryTextPaint { get; set; }

        public TimelineTheme(string userid)
        {
            bkgColor1 = (Xamarin.Forms.Color)App.Current.Resources["bkgColor1"];
            bkgColor2 = (Xamarin.Forms.Color)App.Current.Resources["bkgColor2"];
            bkgColor3 = (Xamarin.Forms.Color)App.Current.Resources["bkgColor3"];
            textColor1 = (Xamarin.Forms.Color)App.Current.Resources["textColor1"];
            textColor2 = (Xamarin.Forms.Color)App.Current.Resources["textColor2"];

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
            HighlightPaint.Color = SKColor.Parse(Preferences.Get("highlight_color", skia.Extensions.ToSKColor(bkgColor3).ToString()));

            EventPaint = new SKPaint();
            EventPaint.Color = SKColor.Parse(Preferences.Get("event_color", skia.Extensions.ToSKColor(bkgColor2).ToString()));
            EventPaint.Style = SKPaintStyle.Fill;

            EventBorderPaint = new SKPaint();
            EventBorderPaint.Color = SKColor.Parse(Preferences.Get("eventborder_color", skia.Extensions.ToSKColor(bkgColor1).ToString()));
            EventBorderPaint.StrokeWidth = 4;
            EventBorderPaint.Style = SKPaintStyle.Stroke;

            EventTextPaint = new SKPaint();
            EventTextPaint.Color = SKColor.Parse(Preferences.Get("eventtext_color", skia.Extensions.ToSKColor(textColor1).ToString()));
            EventTextPaint.TextSize = 48;

            SummaryTextPaint = new SKPaint();
            SummaryTextPaint.Color = SKColor.Parse(Preferences.Get("summarytext_color", skia.Extensions.ToSKColor(textColor1).ToString()));
            SummaryTextPaint.TextSize = 78;
            SummaryTextPaint.TextAlign = SKTextAlign.Center;
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
            Preferences.Set("summarytext_color", SummaryTextPaint.Color.ToString());
        }
    }
}
