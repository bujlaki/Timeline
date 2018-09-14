using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Essentials;
using Acr.UserDialogs;

using Timeline.Models;
using Timeline.Objects.Timeline;
using Timeline.Objects.Collection;


namespace Timeline.ViewModels
{
	public class VMTimeline : Base.VMBase
    {
        private MTimelineInfo timelineInfo;

        public string Title { get { return timelineInfo.Name; } }

        public ObservableCollection<MEventType> EventTypes
        {
            get {
                if(timelineInfo==null) return null;
                return timelineInfo.EventTypes;
            }
        }

        public Dictionary<string, Color> EventTypesDict { get; set; }

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
        public CustomObservableCollection<MTimelineEvent> Events { get; set; }
        public int LaneCount { get; set; }

        private bool eventInfoVisible = false;
        public bool EventInfoVisible {
            get { return eventInfoVisible; }
            set { eventInfoVisible = value; RaisePropertyChanged("EventInfoVisible"); }
        }

        private bool isEditingEventType = false;
        public bool IsEditingEventType
        {
            get { return isEditingEventType; }
            set { isEditingEventType = value; RaisePropertyChanged("IsEditingEventType"); }
        }

        private MTimelineEvent selectedEvent = null;
        public MTimelineEvent SelectedEvent {
            get { return selectedEvent; }
            set { selectedEvent = value;
                RaisePropertyChanged("SelectedEvent");
                RaisePropertyChanged("SelectedEventTimeFrame");
                RaisePropertyChanged("SelectedEventTypeColor");
                RaisePropertyChanged("SelectedEventTypeName");
                RaisePropertyChanged("SelectedEventTitle");
                RaisePropertyChanged("SelectedEventDescription");
                RaisePropertyChanged("SelectedEventImageSource");
            }
        }

        public string SelectedEventTitle {
            get {
                if (selectedEvent == null) return "";
                return selectedEvent.Title;
            }
        }

        public string SelectedEventDescription {
            get {
                if (selectedEvent == null) return "";
                return selectedEvent.Description;
            }
        }

        public ImageSource SelectedEventImageSource {
            get {
                if (selectedEvent == null) return null;
                return selectedEvent.Image.Source;
            }
        }

        private MEventType selectedEventType;
        public MEventType SelectedEventType {
            get {
                if (selectedEvent == null) return null;
                return selectedEventType;
            }
            set { selectedEventType = value; RaisePropertyChanged("SelectedEventType"); }
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

        private string selectedEventTypeName;
        public string SelectedEventTypeName
        {
            get {
                if (SelectedEvent == null) return "";
                return selectedEventTypeName;
            }
            set { selectedEventTypeName = value; RaisePropertyChanged("SelectedEventTypeName"); }
        }

        public Color selectedEventTypeColor;
        public Color SelectedEventTypeColor
        {
            get {
                if (SelectedEvent == null) return Color.Black;
                return selectedEventTypeColor;
            }
            set { selectedEventTypeColor = value; RaisePropertyChanged("SelectedEventTypeColor"); }
        }

        //commands
        public Command CmdTap { get; set; }
        public Command CmdAddEvent { get; set; }
        public Command CmdCloseEventInfo { get; set; }
        public Command CmdEditEventInfo { get; set; }
        public Command CmdDeleteEvent { get; set; }
        public Command CmdPickType { get; set; }
        public Command CmdSetEventType { get; set; }
        public Command CmdCancelEventType { get; set; }
        public Command CmdGenerateEvents { get; set; }

        public VMTimeline() : base()
        {
            CmdTap = new Command(TapExecute);
            CmdAddEvent = new Command(CmdAddEventExecute);
            CmdCloseEventInfo = new Command(CmdCloseEventInfoExecute);
            CmdEditEventInfo = new Command(CmdEditEventInfoExecute);
            CmdDeleteEvent = new Command(CmdDeleteEventExecute);
            CmdPickType = new Command(CmdPickEventTypeExecute);
            CmdSetEventType = new Command(CmdSetEventTypeExecute);
            CmdCancelEventType = new Command(CmdCancelEventTypeExecute);
            CmdGenerateEvents = new Command(CmdGenerateEventsExecute);

            Events = new CustomObservableCollection<MTimelineEvent>();
            EventTypesDict = new Dictionary<string, Color>();
            EventTypesDict.Add("Default", (Xamarin.Forms.Color)App.Current.Resources["bkgColor1"]);

            Date = new TimelineDateTime();
            EventInfoVisible = false;
            IsEditingEventType = false;
            
            //subscribe to events
            MessagingCenter.Subscribe<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_created", TimelineEvent_created);
            MessagingCenter.Subscribe<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_updated", TimelineEvent_updated);
            MessagingCenter.Subscribe<VMGenerateEvents, MTimelineInfo>(this, "TimelineEvents_generated", TimelineEvents_generated);
        }

        public void SetModel(MTimelineInfo model)
        {
            timelineInfo = model;
            Events.Clear();
            LoadEvents(timelineInfo.TimelineId);

            SelectedEvent = null;
            IsEditingEventType = false;

            //setup eventtypes
            EventTypesDict.Clear();
            foreach (MEventType etype in timelineInfo.EventTypes) EventTypesDict.Add(etype.TypeName, etype.Color);

            ZoomUnit = TimelineUnits.Year;

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
            //Events.Add(arg2);

            App.services.Database.UpdateEvent(arg2);
            RaisePropertyChanged("ItemsSource");

            RaisePropertyChanged("SelectedEvent");
            RaisePropertyChanged("SelectedEventTimeFrame");
            RaisePropertyChanged("SelectedEventTypeColor");
            RaisePropertyChanged("SelectedEventTypeName");
            RaisePropertyChanged("SelectedEventTitle");
            RaisePropertyChanged("SelectedEventDescription");
            RaisePropertyChanged("SelectedEventImageSource");
        }

        private void TimelineEvents_generated(VMGenerateEvents arg1, MTimelineInfo arg2)
        {
            Events.Clear();
            LoadEvents(arg2.TimelineId);
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
            SelectedEventType = EventTypes.FirstOrDefault(x => x.TypeName == SelectedEvent.EventType);
            SelectedEventTypeName = SelectedEvent.EventType;
            SelectedEventTypeColor = SelectedEventType.Color;
            EventInfoVisible = true;
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

        private void CmdPickEventTypeExecute(object obj)
        {
            IsEditingEventType = true;
        }

        private void CmdSetEventTypeExecute(object obj)
        {
            SelectedEvent.EventType = SelectedEventType.TypeName;
            IsEditingEventType = false;
            SelectedEventTypeName = SelectedEventType.TypeName;
            SelectedEventTypeColor = SelectedEventType.Color;
            SelectedEventType = null;
            App.services.Database.UpdateEvent(SelectedEvent);
            Events.ReportItemChange(SelectedEvent);
        }

        private void CmdCancelEventTypeExecute(object obj)
        {
            IsEditingEventType = false;
        }

        private void CmdGenerateEventsExecute(object obj)
        {
            App.services.Navigation.GoToGenerateEventsPage(timelineInfo);
        }

        public void LoadEvents(string id)
        {
            if (id == "") return;
            Task.Run(async () => {
                Events = new CustomObservableCollection<MTimelineEvent>(await App.services.Database.GetEvents(id));
                LaneCount = EventManager.SortEventsToLanes(Events, 10);
                RaisePropertyChanged("Events");
            });
        }
    }
}
