using System;
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
        private MTimelineInfo timelineInfo;

        public string Title { get { return timelineInfo.Name; } }

        public Dictionary<string, Color> EventTypes { get { return timelineInfo.EventTypes; } }

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
        public ObservableCollection<MTimelineEvent> Events { get; set; }
        public int LaneCount { get; set; }

        private bool eventInfoVisible = false;
        public bool EventInfoVisible {
            get { return eventInfoVisible; }
            set { eventInfoVisible = value; RaisePropertyChanged("EventInfoVisible"); }
        }

        private MTimelineEvent selectedEvent = null;
        public MTimelineEvent SelectedEvent {
            get { return selectedEvent; }
            set { selectedEvent = value;
                RaisePropertyChanged("SelectedEvent");
                RaisePropertyChanged("SelectedEventTimeFrame");
                RaisePropertyChanged("SelectedEventTypeColor");
                RaisePropertyChanged("SelectedEventTypeName");
            }
        }

        public string SelectedEventTimeFrame {
            get
            {
                if (SelectedEvent == null) return "";
                if (SelectedEvent.EndDateSet)
                    return "( " + SelectedEvent.StartDate.DateStr() + " - " + SelectedEvent.EndDate.DateStr() + " )";
                else
                    return "( " + SelectedEvent.StartDate.DateStr() + " )";
            }
        }

        public string SelectedEventTypeName
        {
            get {
                if (SelectedEvent == null) return "";
                return SelectedEvent.EventType;
            }
        }

        public Color SelectedEventTypeColor
        {
            get {
                if (SelectedEvent == null) return Color.Black;
                return EventTypes[SelectedEvent.EventType];
            }
        }

        //commands
        public Command CmdTap { get; set; }
        public Command CmdLongTap { get; set; }
        public Command CmdAddEvent { get; set; }
        public Command CmdOptions { get; set; }
        public Command CmdCloseEventInfo { get; set; }
        public Command CmdEditEventInfo { get; set; }
        public Command CmdDeleteEvent { get; set; }

        public VMTimeline() : base()
        {
            CmdTap = new Command(TapExecute);
            CmdLongTap = new Command(LongTapExecute);
            CmdAddEvent = new Command(CmdAddEventExecute);
            CmdOptions = new Command(CmdOptionsExecute);
            CmdCloseEventInfo = new Command(CmdCloseEventInfoExecute);
            CmdEditEventInfo = new Command(CmdEditEventInfoExecute);
            CmdDeleteEvent = new Command(CmdDeleteEventExecute);

            Events = new ObservableCollection<MTimelineEvent>();
            Date = new TimelineDateTime();
            EventInfoVisible = false;
            
            //subscribe to events
            MessagingCenter.Subscribe<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_created", TimelineEvent_created);
            MessagingCenter.Subscribe<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_updated", TimelineEvent_updated);
        }

        public void SetModel(MTimelineInfo model)
        {
            timelineInfo = model;
            LoadEvents(timelineInfo.TimelineId);
            UpdateAllProperties();
        }

        private void TimelineEvent_created(VMTimelineEvent arg1, MTimelineEvent arg2)
        {
            arg2.TimelineId = timelineInfo.TimelineId;
            Events.Add(arg2);
            EventManager.SortEventsToLanes(Events, 10);

            App.services.Database.StoreEvent(arg2);
            RaisePropertyChanged("ItemsSource");
        }

        private void TimelineEvent_updated(VMTimelineEvent arg1, MTimelineEvent arg2)
        {
            arg2.TimelineId = timelineInfo.TimelineId;
            Events.Add(arg2);

            App.services.Database.UpdateEvent(arg2);
            RaisePropertyChanged("ItemsSource");
        }

        private void TapExecute(object obj)
        {
            TapEventArg arg = (TapEventArg)obj;

            //tapped event
            MTimelineEvent tlevent = EventManager.GetEventAt(Events, arg.Lane, arg.Ticks);
            if (tlevent == null) return;
            if (tlevent.Image == null)
            {
                tlevent.Image = new Image();
                tlevent.ClearImage();
            }

            SelectedEvent = tlevent;
            EventInfoVisible = true;
        }

        private void LongTapExecute(object obj)
        {
            TapEventArg arg = (TapEventArg)obj;

            //new event
            TimelineDateTime tld = TimelineDateTime.FromTicks(arg.Ticks);
            tld.Precision = arg.ZoomUnit - 1;
            
            MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(new MTimelineEvent("", tld)));
        }

        private void CmdAddEventExecute(object obj)
        {
            //new event
            TimelineDateTime tld = TimelineDateTime.FromTicks(Date.Ticks);
            tld.Precision = ZoomUnit - 1;

            MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(new MTimelineEvent("", tld)));
        }

        private void CmdOptionsExecute(object obj)
        {
            //new event
            TimelineDateTime tld = TimelineDateTime.FromTicks(Date.Ticks);
            tld.Precision = ZoomUnit - 1;

            MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(new MTimelineEvent("", tld)));
        }

        private void CmdCloseEventInfoExecute(object obj)
        {
            EventInfoVisible = false;
            SelectedEvent = null;
        }

        private void CmdEditEventInfoExecute(object obj)
        {
            if(SelectedEvent != null)
                MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(SelectedEvent));
        }

        private void CmdDeleteEventExecute(object obj)
        {
            if (SelectedEvent != null)
            {
                Task.Run(async () =>
                {
                    ConfirmConfig cc = new ConfirmConfig();
                    cc.Message = "Are you sure?";
                    cc.Title = "Delete event";
                    if(await UserDialogs.Instance.ConfirmAsync(cc))
                    {
                        await App.services.Database.DeleteEvent(SelectedEvent);
                        MainThread.BeginInvokeOnMainThread(() => {
                            Events.Remove(SelectedEvent);
                            EventInfoVisible = false;
                            SelectedEvent = null;
                            RaisePropertyChanged("ItemsSource");
                        });
                    }
                });
            }
        }

        public void LoadEvents(string id)
        {
            if (id == "") return;
            Task.Run(async () => {
                Events = new ObservableCollection<MTimelineEvent>(await App.services.Database.GetEvents(id));
                LaneCount = EventManager.SortEventsToLanes(Events, 10);
                RaisePropertyChanged("Events");
            });
        }
    }
}
