using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Reflection;
using System.Threading.Tasks;

namespace Timeline.Services
{
    class Storage : IStorage
    {
        public void DownloadPiktograms()
        {
            string appData = FileSystem.AppDataDirectory;
            if (Directory.Exists(appData + "/pictograms") == false)
            {
                DirectoryInfo piktoDir = Directory.CreateDirectory(appData + "/pictograms");

                Console.WriteLine("FULL PATH: " + piktoDir.FullName);

                Task.Run(async () => await DownloadPiktogram("Timeline.Embedded.Piktograms.birth128.png", piktoDir, "birth.png")).Wait();
                Task.Run(async () => await DownloadPiktogram("Timeline.Embedded.Piktograms.crown128.png", piktoDir, "crown.png")).Wait();
                Task.Run(async () => await DownloadPiktogram("Timeline.Embedded.Piktograms.rip128.png", piktoDir, "rip.png")).Wait();
                Task.Run(async () => await DownloadPiktogram("Timeline.Embedded.Piktograms.war128.png", piktoDir, "war.png")).Wait();
            }

            string[] files = Directory.GetFiles(appData + "/pictograms");
        }

        private async Task DownloadPiktogram(string filename, DirectoryInfo dirInfo, string saveas)
        {
            byte[] buffer;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(filename))
            {
                long length = s.Length;
                buffer = new byte[length];
                s.Read(buffer, 0, (int)length);
            }

            filename = dirInfo.FullName + "/" + saveas;
            using (var stream = new FileStream(filename, FileMode.Create))
            {
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }

        public List<ImageSource> LoadPiktograms()
        {
            throw new NotImplementedException();
        }
    }
}
