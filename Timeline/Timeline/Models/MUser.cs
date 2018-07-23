using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Timeline.Objects.Auth;
using Xamarin.Auth;

namespace Timeline.Models
{
    public class MUser
    {

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public ObservableCollection<MTimelineInfo> Timelines { get; set; }

        public MUser()
        {
            Timelines = new ObservableCollection<MTimelineInfo>();
            Clear();
        }

        public void Clear()
        {
            UserId = "";
            UserName = "";
            Email = "";
            PhotoUrl = "";
        }
            
    }
}
