using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Timeline.Models;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using Timeline.Objects.Timeline;

namespace Timeline.ViewModels
{
    public class VMGenerateEvents : Base.VMBase
    {
        private MTimelineInfo tlinfo;
        private HttpClient client;

        public Command CmdGenerate { get; set; }

        //SEGMENTS
        public int SelectedSegment { get; set; }
        public bool ShowSegment1 { get; set; }
        public bool ShowSegment2 { get; set; }
        public bool ShowSegment3 { get; set; }
        public Command CmdTabSegmentTap { get; set; }

        //SEGMENT1 - www.vizgr.org search
        private string startDateStr;
        public string StartDateStr { get { return startDateStr; } set { startDateStr = value; RaisePropertyChanged("StartDateStr"); } }
        private string endDateStr;
        public string EndDateStr { get { return endDateStr; } set { endDateStr = value; RaisePropertyChanged("EndDateStr"); } }
        private string queryText;
        public string QueryText { get { return queryText; } set { queryText = value; RaisePropertyChanged("QueryText"); } }
        private string commonTitle;
        public string CommonTitle { get { return commonTitle; } set { commonTitle = value; RaisePropertyChanged("CommonTitle"); } }
        
        public VMGenerateEvents()
        {
            CmdGenerate = new Command(CmdGenerateExecute);
            CmdTabSegmentTap = new Command(CmdTabSegmentTapExecute);
        }

        public void InitView(MTimelineInfo _tlinfo)
        {
            tlinfo = _tlinfo;

            SelectedSegment = 0;
            StartDateStr = "";
            EndDateStr = "";
            QueryText = "";
            CommonTitle = "";
            ShowSegment1 = true;
            ShowSegment2 = false;
            ShowSegment3 = false;

            RaisePropertyChanged("ShowSegment1");
            RaisePropertyChanged("ShowSegment2");
            RaisePropertyChanged("ShowSegment3");
        }

        private void CmdTabSegmentTapExecute(object obj)
        {
            ShowSegment1 = false;
            ShowSegment2 = false;
            ShowSegment3 = false;
            switch(SelectedSegment)
            {
                case 0: ShowSegment1 = true; break;
                case 1: ShowSegment2 = true; break;
                case 2: ShowSegment3 = true; break;
            }
            RaisePropertyChanged("ShowSegment1");
            RaisePropertyChanged("ShowSegment2");
            RaisePropertyChanged("ShowSegment3");
        }

        private void CmdGenerateExecute(object obj)
        {
            if (StartDateStr == "") { UserDialogs.Instance.Alert("Please set start date in YYYYMMDD format!"); return; }
            if (EndDateStr == "") { UserDialogs.Instance.Alert("Please set end date in YYYYMMDD format!"); return; }
            if (CommonTitle == "") { UserDialogs.Instance.Alert("Please set the common title for the events!"); return; }

            string url;
            url = "http://www.vizgr.org/historical-events/search.php?granularity=all&begin_date=" + StartDateStr + "&end_date=" + EndDateStr;

            if (QueryText != "") url += "&query=" + QueryText;

            Uri uri = new Uri(url);
            var req = HttpWebRequest.Create(uri);
            req.Method = "GET";
            req.ContentType = "application/json";

            using (HttpWebResponse response = req.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    UserDialogs.Instance.Alert("Error gettind data. Server response: " + response.StatusCode.ToString());
                    return;
                }
                else
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        if(string.IsNullOrWhiteSpace(content) || content== "No events found for this query.")
                        {
                            UserDialogs.Instance.Alert("Response contained empty body...");
                            return;
                        }
                        else
                        {
                            GenerateEventsFromXML(content);
                        }
                    }
                }
            }
        }

        private void GenerateEventsFromXML(string xmlStr)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);

            XmlNode node;
            node = doc.SelectSingleNode("result/count");

            Task.Run(async () =>
            {
                ConfirmConfig cc = new ConfirmConfig();
                cc.Message = "Creating events from " + node.InnerText + " results.";
                if (await UserDialogs.Instance.ConfirmAsync(cc))
                {
                    XmlNodeList eventNodes = doc.SelectNodes("result/event");
                    List<MTimelineEvent> events = new List<MTimelineEvent>();
                    foreach (XmlNode eventNode in eventNodes)
                    {
                        TimelineDateTime tld = TLDParse(eventNode.SelectSingleNode("date").InnerText);
                        MTimelineEvent e = new MTimelineEvent(CommonTitle, tld);
                        e.Description = eventNode.SelectSingleNode("description").InnerText;
                        e.TimelineId = tlinfo.TimelineId;

                        events.Add(e);
                    }

                    await App.services.Database.StoreEvents(events);

                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => 
                    {
                        MessagingCenter.Send<VMGenerateEvents, MTimelineInfo>(this, "TimelineEvents_generated", tlinfo);
                        App.services.Navigation.GoBack();
                    });

                }
            });

        }

        private TimelineDateTime TLDParse(string dateStr)
        {
            try
            {
                if (dateStr.Length <= 4) return new TimelineDateTime(int.Parse(dateStr));

                string[] parts = dateStr.Split('/');
                if (parts.Length == 3) return new TimelineDateTime(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
                if (parts.Length == 2) return new TimelineDateTime(int.Parse(parts[0]), int.Parse(parts[1]));

                return null;
            } catch(Exception ex)
            {
                Console.WriteLine("TLDParse error: " + ex.Message);
                return null;
            }
        }
    }
}
