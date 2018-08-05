using System;
using System.Collections.Generic;
using System.ComponentModel;

using Timeline.Services.Base;

namespace Timeline.ViewModels.Base
{
    public class VMBase : INotifyPropertyChanged
    {
        private object syncLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool busy = false;
        public bool Busy {
            get { return busy; }
            set { busy = value; RaisePropertyChanged("Busy"); }
        }

        private string busyMessage = "";
        public string BusyMessage
        {
            get { return busyMessage; }
            set { busyMessage = value; RaisePropertyChanged("BusyMessage"); }
        }

        public VMBase()
        {
            Busy = false;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void UpdateAllProperties()
        {
            RaisePropertyChanged(String.Empty);
        }

        protected bool Lock()
        {
            lock (syncLock)
            {
                if (busy) return false;
                Busy = true;
                return true;
            }
        }

        protected void Unlock()
        {
            lock (syncLock)
            {
                Busy = false;
            }
        }
    }
}
