using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

using Xamarin.Essentials;

namespace Timeline.ViewModels
{
    public class VMPictograms : Base.VMBase
    {
        public List<Image> Pictograms { get; set; }

        public Command CmdImageTap { get; set; }

        public VMPictograms() : base()
        {
            Pictograms = new List<Image>();
            CmdImageTap = new Command(CmdImageTapExecute);
        }

        private void CmdImageTapExecute(object obj)
        {
            string fname = obj.ToString();
            MessagingCenter.Send<VMPictograms, string>(this, "Pictogram_selected", fname);
            App.services.Navigation.GoBack();
        }

        public void LoadPictograms()
        {
            string appData = FileSystem.AppDataDirectory;

            //download pictograms
            if (!Directory.Exists(appData + "/pictograms")) { App.services.Storage.DownloadPiktograms(); }

            string[] files = Directory.GetFiles(FileSystem.AppDataDirectory + "/pictograms");

            foreach (string f in files)
            {
                Image img = new Image();
                img.Source = ImageSource.FromFile(f);
                img.WidthRequest = 128;
                img.HeightRequest = 128;

                TapGestureRecognizer tapper = new TapGestureRecognizer();
                tapper.Command = CmdImageTap;
                tapper.CommandParameter = f;
                img.GestureRecognizers.Add(tapper);

                Pictograms.Add(img);
            }

            RaisePropertyChanged("Pictograms");
        }
    }
}
