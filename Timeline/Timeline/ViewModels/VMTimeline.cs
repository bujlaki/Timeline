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
        private string title = "";
        public string Title {
            get { return title; }
            set { title = value; RaisePropertyChanged("Title"); }
        }

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

        private EventTree eventTree;
        public EventTree EventTree
        {
            get { return eventTree; }
            set { eventTree = value; RaisePropertyChanged("EventTree"); }
        }

        public Int64 Pixeltime { get; }

        public string TimelineId { get; set; }
        public ObservableCollection<MTimelineEvent> Events { get; set; }
        public int LaneCount { get; set; }

        private bool eventInfoVisible = false;
        public bool EventInfoVisible {
            get { return eventInfoVisible; }
            set { eventInfoVisible = value; RaisePropertyChanged("EventInfoVisible"); }
        }

        private MTimelineEvent eventSelected = null;
        public MTimelineEvent EventSelected {
            get { return eventSelected; }
            set { eventSelected = value; RaisePropertyChanged("EventSelected"); }
        }

        public Command CmdTap { get; set; }
        public Command CmdLongTap { get; set; }
        public Command CmdAddEvent { get; set; }
        public Command CmdCloseEventInfo { get; set; }
        public Command CmdEditEventInfo { get; set; }
        public Command CmdDeleteEvent { get; set; }

        public VMTimeline() : base()
        {
            CmdTap = new Command(TapExecute);
            CmdLongTap = new Command(LongTapExecute);
            CmdAddEvent = new Command(CmdAddEventExecute);
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

        private void TimelineEvent_created(VMTimelineEvent arg1, MTimelineEvent arg2)
        {
            arg2.TimelineId = this.TimelineId;
            Events.Add(arg2);
            EventManager.SortEventsToLanes(Events, 10);

            App.services.Database.StoreEvent(arg2);
            RaisePropertyChanged("ItemsSource");
        }

        private void TimelineEvent_updated(VMTimelineEvent arg1, MTimelineEvent arg2)
        {
            arg2.TimelineId = this.TimelineId;
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

            EventSelected = tlevent;
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

        private void CmdCloseEventInfoExecute(object obj)
        {
            EventInfoVisible = false;
            EventSelected = null;
        }

        private void CmdEditEventInfoExecute(object obj)
        {
            if(EventSelected!=null)
                MainThread.BeginInvokeOnMainThread(() => App.services.Navigation.GoToTimelineEventView(EventSelected));
        }

        private void CmdDeleteEventExecute(object obj)
        {
            if (EventSelected != null)
            {
                App.services.Database.DeleteEvent(EventSelected);
                Events.Remove(EventSelected);
                EventInfoVisible = false;
                EventSelected = null;
                RaisePropertyChanged("ItemsSource");
            }
        }

        public void LoadEvents()
        {
            if (TimelineId == "") return;
            Task.Run(async () => {
                Events = new ObservableCollection<MTimelineEvent>(await App.services.Database.GetEvents(TimelineId));
                LaneCount = EventManager.SortEventsToLanes(Events, 10);
                EventTree = EventManager.BuildEventTree(Events);
                RaisePropertyChanged("Events");
            });
        }
    }
}
