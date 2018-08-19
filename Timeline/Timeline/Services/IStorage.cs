using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Timeline.Services
{
    public interface IStorage
    {
        void DownloadPiktograms();
        List<ImageSource> LoadPiktograms();
    }
}
