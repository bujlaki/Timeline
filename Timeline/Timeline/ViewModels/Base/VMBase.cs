﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

using Timeline.Services.Base;

namespace Timeline.ViewModels.Base
{
    public class VMBase : INotifyPropertyChanged
    {
        private object syncLock = new object();
        private bool busy = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy {
            get { return busy; }
            set { busy = value; RaisePropertyChanged("Busy"); }
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
