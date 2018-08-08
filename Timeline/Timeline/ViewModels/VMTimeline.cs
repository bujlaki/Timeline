﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;
using Acr.UserDialogs;

using Timeline.Models;
using Timeline.Objects.Timeline;


namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
        private TimelineUnits zoomUnit;
        public TimelineUnits ZoomUnit
        {
            get { return zoomUnit; }
            set { zoomUnit = value; RaisePropertyChanged("ZoomUnit"); }
        }

        private TimelineDateTime date;
        public TimelineDateTime Date
        {
            get { return date; }
            set { date = value; RaisePropertyChanged("Date"); }
        }

        public Int64 Pixeltime { get; }

        public string TimelineId { get; set; }
        public ObservableCollection<MTimelineEvent> Events { get; set; }
        public int LaneCount { get; set; }

        public Command CmdTap { get; set; }
        public Command CmdLongTap { get; set; }

        public VMTimeline() : base()
        {
            CmdTap = new Command(TapExecute);
            CmdLongTap = new Command(LongTapExecute);
            Events = new ObservableCollection<MTimelineEvent>();

            //subscribe to events
            MessagingCenter.Subscribe<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_created", TimelineEvent_created);
        }

        private void TimelineEvent_created(VMTimelineEvent arg1, MTimelineEvent arg2)
        {
            arg2.TimelineId = this.TimelineId;
            Events.Add(arg2);

            App.services.Database.StoreEvent(arg2);
            Console.WriteLine("TimelineEvent created");
        }

        private void TapExecute(object obj)
        {
            TapEventArg arg = (TapEventArg)obj;

            //tapped event
            MTimelineEvent tlevent = EventManager.GetEventAt(Events, arg.Lane, arg.Ticks);
            if (tlevent == null) return;

            MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(tlevent));
        }

        private void LongTapExecute(object obj)
        {
            TapEventArg arg = (TapEventArg)obj;

            //new event
            TimelineDateTime tld = TimelineDateTime.FromTicks(arg.Ticks);
            tld.Precision = arg.ZoomUnit - 1;
            
            MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(new MTimelineEvent("new event", tld)));
        }

        public void LoadEvents()
        {
            if (TimelineId == "") return;
            Task.Run(async () => {
                Events = new ObservableCollection<MTimelineEvent>(await App.services.Database.GetEvents(TimelineId));
                LaneCount = EventManager.SortEventsToLanes(Events, 10);
                RaisePropertyChanged("Events");
            });
        }
    }
}
