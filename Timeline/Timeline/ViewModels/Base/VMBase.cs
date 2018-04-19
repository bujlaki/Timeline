using System;
using System.Collections.Generic;
using System.ComponentModel;

using Timeline.Services.Base;

namespace Timeline.ViewModels.Base
{
    public class VMBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected ServiceContainer _services;

        public VMBase(ServiceContainer services)
        {
            _services = services;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void UpdateAll()
        {
            RaisePropertyChanged(String.Empty);
        }
    }
}
