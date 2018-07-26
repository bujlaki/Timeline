using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Timeline.ViewModels
{
    public class VMTimelineEvent : Base.VMBase
    {
        public Command CmdCreate { get; set; }
        public VMTimelineEvent Event { get; set; }

        //DATE ENTRY
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
        private int digit4;
        public int Digit1
        {
            get { return digit1; }
            set { digit1 = value; RaisePropertyChanged("Digit1"); }
        }
        public int Digit2
        {
            get { return digit2; }
            set { digit2 = value; RaisePropertyChanged("Digit2"); }
        }
        public int Digit3
        {
            get { return digit3; }
            set { digit3 = value; RaisePropertyChanged("Digit3"); }
        }
        public int Digit4
        {
            get { return digit4; }
            set { digit4 = value; RaisePropertyChanged("Digit4"); }
        }
        public Command CmdDigitTapped { get; set; }

        public ObservableCollection<string> Months { get; set; }

        public VMTimelineEvent() : base()
        {
            CmdCreate = new Command(CmdCreateExecute);

            CmdBCAC = new Command(CmdBCACExecute);

            CmdDigitTapped = new Command(CmdDigitTappedExecute);

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
        }

        private void CmdDigitTappedExecute(object obj)
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
        }

        private void CmdBCACExecute(object obj)
        {
            BCAC = bcacText == "AC" ? "BC" : "AC";
        }

        private void CmdCreateExecute(object obj)
        {

            App.services.Navigation.GoBack();
        }
    }
}
