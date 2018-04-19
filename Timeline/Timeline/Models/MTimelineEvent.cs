using System;
using Xamarin.Forms;

namespace Timeline.Models
{
    public class MTimelineEvent
    {
        public string Title
        {
            get;
            set;
        }

        public Uri Link
        {
            get;
            set;
        }

        public Image Image
        {
            get;
            set;
        }

        public MTimelineEvent()
        {
        }

    }
}
