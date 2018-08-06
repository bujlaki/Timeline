using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

using Timeline.Services.Base;

namespace Timeline.ViewModels.Base
{
    public class VMBase : INotifyPropertyChanged, INotifyCollectionChanged
    {
        private object syncLock = new object();

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void UpdateAllProperties()
        {
            RaisePropertyChanged(String.Empty);
        }

        public void RaiseCollectionItemChanged(NotifyCollectionChangedAction action, object item = null)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item));
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
