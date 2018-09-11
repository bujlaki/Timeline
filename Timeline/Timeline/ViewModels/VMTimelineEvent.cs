using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

using Plugin.Media;
using Acr.UserDialogs;

using Timeline.Models;
using Timeline.Objects.Timeline;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using System.IO;

namespace Timeline.ViewModels
{
    public class VMTimelineEvent : Base.VMBase
    {
        private Color EnabledColor = Color.OrangeRed;
        private Color DisabledColor = Color.Gray;

        public Command CmdStartDate { get; set; }
        public Command CmdEndDate { get; set; }
        public Command CmdClearEndDate { get; set; }
        public Command CmdSetDate { get; set; }
        public Command CmdPickerLabelTap { get; set; }
        public Command CmdImage { get; set; }
        public Command CmdPictogram { get; set; }
        public Command CmdCreate { get; set; }
        public Command CmdUpdate { get; set; }
        public Command CmdTabSegmentTap { get; set; }

        public string PageTitle { get; set; }
        public bool IsNewEvent { get; set; }
        public int SelectedSegment { get; set; }
        public bool ShowSegment1 { get; set; }
        public bool ShowSegment2 { get; set; }

        private MTimelineEvent tlevent;
        public MTimelineEvent Event {
            get { return tlevent; }
            set { tlevent = value; UpdateAllProperties(); }
        }

        public string StartDateStr
        {
            get { return Event.StartDate.DateStr(); }
        }

        public string EndDateStr
        {
            get {
                if (!Event.EndDateSet) return "";
                return Event.EndDate.DateStr();
            }
        }

        public ImageSource EventImageSource
        {
            get
            {
                if (Event.Image == null) return null;
                return Event.Image.Source;
            }
        }

        private bool datePickerVisible;
        public bool DatePickerVisible
        {
            get { return datePickerVisible; }
            set { datePickerVisible = value; RaisePropertyChanged("DatePickerVisible"); }
        }

        //DATE ENTRY
        private bool settingStartDate;

        //---BCAC---
        private string bcacText;
        public string BCAC {
            get { return bcacText; }
            set { bcacText = value; RaisePropertyChanged("BCAC"); }
        }
        public Command CmdBCAC { get; set; }

        //---YEAR---
        private int digit1;
        private int digit2;
        private int digit3;
        private int digit4 = 1;
        private int currentYear = 1;
        public int Digit1
        {
            get { return digit1; }
            set { digit1 = value; RaisePropertyChanged("Digit1"); UpdateYear(); }
        }
        public int Digit2
        {
            get { return digit2; }
            set { digit2 = value; RaisePropertyChanged("Digit2"); UpdateYear(); }
        }
        public int Digit3
        {
            get { return digit3; }
            set { digit3 = value; RaisePropertyChanged("Digit3"); UpdateYear(); }
        }
        public int Digit4
        {
            get { return digit4; }
            set { digit4 = value; RaisePropertyChanged("Digit4"); UpdateYear(); }
        }
        public Command CmdDigitUp { get; set; }
        public Command CmdDigitDown { get; set; }

        //---MONTH---
        private Color monthLabelColor;
        public Color MonthLabelColor {
            get { return monthLabelColor; }
            set { monthLabelColor = value; RaisePropertyChanged("MonthLabelColor"); }
        }
        private bool monthPickerVisible;
        public bool MonthPickerVisible {
            get { return monthPickerVisible; }
            set {
                monthPickerVisible = value;
                RaisePropertyChanged("MonthPickerVisible");
                MonthLabelColor = monthPickerVisible ? EnabledColor : DisabledColor;
                if (!monthPickerVisible) DayPickerVisible = false;
            }
        }
        private int selectedMonth;
        public int SelectedMonth
        {
            get { return selectedMonth; }
            set { selectedMonth = value; RaisePropertyChanged("SelectedMonth"); UpdateDays(); }
        }
        public ObservableCollection<string> Months { get; set; }

        //---DAY---
        private Color dayLabelColor;
        public Color DayLabelColor {
            get { return dayLabelColor; }
            set { dayLabelColor = value; RaisePropertyChanged("DayLabelColor"); }
        }
        private bool dayPickerVisible;
        public bool DayPickerVisible {
            get { return dayPickerVisible; }
            set {
                dayPickerVisible = value;
                RaisePropertyChanged("DayPickerVisible");
                DayLabelColor = dayPickerVisible ? EnabledColor : DisabledColor;
                if (!dayPickerVisible) HourPickerVisible = false;
                else MonthPickerVisible = true;
            }
        }
        private int daysInMonth;
        public int DaysInMonth
        {
            get { return daysInMonth; }
            set { daysInMonth = value; RaisePropertyChanged("DaysInMonth"); }
        }
        private int selectedDay = 1;
        public int SelectedDay
        {
            get { return selectedDay; }
            set { selectedDay = value; RaisePropertyChanged("SelectedDay"); }
        }

        //---HOUR---
        private Color hourLabelColor;
        public Color HourLabelColor {
            get { return hourLabelColor; }
            set { hourLabelColor = value; RaisePropertyChanged("HourLabelColor"); }
        }
        private bool hourPickerVisible;
        public bool HourPickerVisible {
            get { return hourPickerVisible; }
            set {
                hourPickerVisible = value;
                RaisePropertyChanged("HourPickerVisible");
                HourLabelColor = hourPickerVisible ? EnabledColor : DisabledColor;
                if (!hourPickerVisible) MinutePickerVisible = false;
                else DayPickerVisible = true;
            }
        }
        private int selectedHour;
        public int SelectedHour
        {
            get { return selectedHour; }
            set { selectedHour = value; RaisePropertyChanged("SelectedHour"); }
        }

        //---MINUTE---
        private Color minuteLabelColor;
        public Color MinuteLabelColor {
            get { return minuteLabelColor; }
            set { minuteLabelColor = value; RaisePropertyChanged("MinuteLabelColor"); }
        }
        private bool minutePickerVisible;
        public bool MinutePickerVisible {
            get { return minutePickerVisible; }
            set {
                minutePickerVisible = value;
                RaisePropertyChanged("MinutePickerVisible");
                MinuteLabelColor = minutePickerVisible ? EnabledColor : DisabledColor;
                if (minutePickerVisible) HourPickerVisible = true;
            }
        }
        private int selectedMinute;
        public int SelectedMinute
        {
            get { return selectedMinute; }
            set { selectedMinute = value; RaisePropertyChanged("SelectedMinute"); }
        }

        public VMTimelineEvent() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);
            CmdUpdate = new Command(CmdUpdateExecute);

            CmdImage = new Command(CmdImageExecute);
            CmdPictogram = new Command(CmdPictogramExecute);
            CmdStartDate = new Command(CmdStartDateExecute);
            CmdEndDate = new Command(CmdEndDateExecute);
            CmdClearEndDate = new Command(CmdClearEndDateExecute);
            CmdSetDate = new Command(CmdSetDateExecute);
            CmdPickerLabelTap = new Command(CmdPickerLabelTapExecute);
            CmdBCAC = new Command(CmdBCACExecute);

            CmdDigitUp = new Command(CmdDigitUpExecute);
            CmdDigitDown = new Command(CmdDigitDownExecute);

            CmdTabSegmentTap = new Command(CmdTabSegmentTapExecute);

            DatePickerVisible = false;

            BCAC = "AC";
            Months = new ObservableCollection<string>();
            Months.Add("January");
            Months.Add("February");
            Months.Add("March");
            Months.Add("April");
            Months.Add("May");
            Months.Add("June");
            Months.Add("July");
            Months.Add("August");
            Months.Add("September");
            Months.Add("October");
            Months.Add("November");
            Months.Add("December");

            MessagingCenter.Subscribe<VMPictograms, string>(this, "Pictogram_selected", Pictogram_selected);
        }

        public void InitView(MTimelineEvent tlevent)
        {
            Event = tlevent;
            ShowSegment1 = true;
            ShowSegment2 = false;
            RaisePropertyChanged("ShowSegment1");
            RaisePropertyChanged("ShowSegment2");
        }

        private void CmdDigitUpExecute(object obj)
        {
            switch(int.Parse(obj.ToString()))
            {
                case 1000:
                    Digit1++;
                    if (Digit1 > 9) Digit1 = 0;
                    break;
                case 100:
                    Digit2++;
                    if (Digit2 > 9) Digit2 = 0;
                    break;
                case 10:
                    Digit3++;
                    if (Digit3 > 9) Digit3 = 0;
                    break;
                case 1:
                    Digit4++;
                    if (Digit4 > 9) Digit4 = 0;
                    break;
            }
            if(Digit1==0 && Digit2==0 && Digit3==0 && Digit4==0) Digit4 = 1;
        }

        private void CmdDigitDownExecute(object obj)
        {
            switch (int.Parse(obj.ToString()))
            {
                case 1000:
                    Digit1--;
                    if (Digit1 < 0) Digit1 = 9;
                    break;
                case 100:
                    Digit2--;
                    if (Digit2 < 0) Digit2 = 9;
                    break;
                case 10:
                    Digit3--;
                    if (Digit3 < 0) Digit3 = 9;
                    break;
                case 1:
                    Digit4--;
                    if (Digit4 < 0) Digit4 = 9;
                    break;
            }
            if (Digit1 == 0 && Digit2 == 0 && Digit3 == 0 && Digit4 == 0) Digit4 = 1;
        }

        private void CmdBCACExecute(object obj)
        {
            BCAC = bcacText == "AC" ? "BC" : "AC";
        }

        private void CmdStartDateExecute(object obj)
        {
            //setup date picker
            settingStartDate = true;
            InitPicker(Event.StartDate);
            DatePickerVisible = true;
        }

        private void CmdEndDateExecute(object obj)
        {
            //setup date picker
            settingStartDate = false;
            InitPicker(Event.EndDate);
            DatePickerVisible = true;
        }

        private void CmdClearEndDateExecute(object obj)
        {
            //setup date picker
            Event.EndDate = TimelineDateTime.AddTo(Event.StartDate);
            Event.EndDateSet = false;
            RaisePropertyChanged("EndDateStr");
        }

        //initialize picker values, and visibility of ValuePicker controls base on TimelineDateTime precision
        private void InitPicker(TimelineDateTime tld)
        {
            SetYearDigits(Math.Abs(tld.Year));
            SelectedMonth = tld.Month - 1;
            SelectedDay = tld.Day;
            SelectedHour = tld.Hour;
            SelectedMinute = tld.Minute;
            BCAC = tld.Year < 0 ? "BC" : "AC";

            switch (tld.Precision)
            {
                case TimelineUnits.Minute:
                    MinutePickerVisible = true;
                    break;
                case TimelineUnits.Hour:
                    HourPickerVisible = true;
                    MinutePickerVisible = false;
                    break;
                case TimelineUnits.Day:
                    DayPickerVisible = true;
                    HourPickerVisible = false;
                    MinutePickerVisible = false;
                    break;
                case TimelineUnits.Month:
                    MonthPickerVisible = true;
                    DayPickerVisible = false;
                    HourPickerVisible = false;
                    MinutePickerVisible = false;
                    break;
                default:
                    MonthPickerVisible = false;
                    break;
            }
        }

        private void CmdSetDateExecute(object obj)
        {
            int year = currentYear;

            if (obj.ToString()=="1") //only when parameter is 1
            {
                if (BCAC == "BC") year = -year;

                if (settingStartDate)
                {
                    Event.StartDate = new TimelineDateTime(year, selectedMonth + 1, selectedDay, selectedHour, selectedMinute);
                    Event.StartDate.Precision = GetDatePrecision();
                    RaisePropertyChanged("StartDateStr");

                    //set end date if it is not set manually
                    if (!Event.EndDateSet) Event.EndDate = TimelineDateTime.AddTo(Event.StartDate);
                }
                else
                {
                    Event.EndDate = new TimelineDateTime(year, selectedMonth + 1, selectedDay, selectedHour, selectedMinute);
                    Event.EndDate.Precision = GetDatePrecision();
                    Event.EndDateSet = true;
                    RaisePropertyChanged("EndDateStr");
                }
            }
            DatePickerVisible = false;
        }

        private void CmdPickerLabelTapExecute(object obj)
        {
            switch(obj.ToString())
            {
                case "1": MonthPickerVisible = !MonthPickerVisible; break;
                case "2": DayPickerVisible = !DayPickerVisible; break;
                case "3": HourPickerVisible = !HourPickerVisible; break;
                case "4": MinutePickerVisible = !MinutePickerVisible; break;
            }
        }

        private async void CmdImageExecute(object obj)
        {
            if(CrossMedia.Current.Initialize().Result)
            {
                if (!CrossMedia.Current.IsPickPhotoSupported) return;

                PickMediaOptions options = new PickMediaOptions();
                options.PhotoSize = PhotoSize.MaxWidthHeight;
                options.MaxWidthHeight = 200;
                MediaFile img = await CrossMedia.Current.PickPhotoAsync(options);

                if (img == null) return;

                var stream = img.GetStream();
                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, (int)stream.Length);
                Event.ImageBase64 = System.Convert.ToBase64String(bytes);

                ImageSource imgSrc;
                imgSrc = ImageSource.FromStream(() => new MemoryStream(bytes));

                stream.Close();
                Event.Image.Source = imgSrc;
                RaisePropertyChanged("EventImageSource");
            }
        }

        private void CmdPictogramExecute(object obj)
        {
            App.services.Navigation.GoToPictogramsView();
        }

        private async void Pictogram_selected(VMPictograms arg1, string arg2)
        {
            string fname = arg2;
            FileStream stream = new FileStream(fname, FileMode.Open);
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes, 0, (int)stream.Length);
            Event.ImageBase64 = System.Convert.ToBase64String(bytes);

            ImageSource imgSrc;
            imgSrc = ImageSource.FromStream(() => new MemoryStream(bytes));

            stream.Close();
            Event.Image.Source = imgSrc;
            RaisePropertyChanged("EventImageSource");
        }

        //set year digits of the picker control, base on the year
        private void SetYearDigits(int year)
        {
            Digit1 = year / 1000;
            year -= Digit1 * 1000;
            Digit2 = year / 100;
            year -= Digit2 * 100;
            Digit3 = year / 10;
            year -= Digit3 * 10;
            Digit4 = year;
        }

        //calculate current year based on the year digits
        private void UpdateYear()
        {
            currentYear = Digit1 * 1000 + Digit2 * 100 + Digit3 * 10 + Digit4;
            UpdateDays();
        }

        //calculate the number of days in the current month
        private void UpdateDays()
        {
            if (currentYear < 1) return;
            DaysInMonth = DateTime.DaysInMonth(currentYear, selectedMonth + 1);
        }

        //returns precision based on picker values
        private TimelineUnits GetDatePrecision()
        {
            if (minutePickerVisible) return TimelineUnits.Minute;
            if (hourPickerVisible) return TimelineUnits.Hour;
            if (dayPickerVisible) return TimelineUnits.Day;
            if (monthPickerVisible) return TimelineUnits.Month;
            return TimelineUnits.Year;
        }
            

        private void CmdCreateExecute(object obj)
        {
            //check if field values are correct
            if (Event.Title == "")
            {
                ToastConfig tc = new ToastConfig("Please set the title");
                tc.Duration = TimeSpan.FromSeconds(2);
                tc.BackgroundColor = (Color)App.Current.Resources["textColor1"];
                tc.MessageTextColor = (Color)App.Current.Resources["bkgColor1"];
                UserDialogs.Instance.Toast(tc);
                return;
            }

            MessagingCenter.Send<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_created", this.Event);
            App.services.Navigation.GoBack();
        }

        private void CmdUpdateExecute(object obj)
        {
            //check if field values are correct
            if (Event.Title == "")
            {
                ToastConfig tc = new ToastConfig("Please set the title");
                tc.Duration = TimeSpan.FromSeconds(2);
                tc.BackgroundColor = (Color)App.Current.Resources["textColor1"];
                tc.MessageTextColor = (Color)App.Current.Resources["bkgColor1"];
                UserDialogs.Instance.Toast(tc);
                return;
            }

            MessagingCenter.Send<VMTimelineEvent, MTimelineEvent>(this, "TimelineEvent_updated", this.Event);
            App.services.Navigation.GoBack();
        }

        private void CmdTabSegmentTapExecute(object obj)
        {
            if (SelectedSegment == 0)
            {
                ShowSegment1 = true;
                ShowSegment2 = false;
            }else
            {
                ShowSegment1 = false;
                ShowSegment2 = true;
            }
            RaisePropertyChanged("ShowSegment1");
            RaisePropertyChanged("ShowSegment2");
        }
    }
}
