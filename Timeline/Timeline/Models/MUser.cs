using Amazon.CognitoIdentity;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Timeline.Objects.Auth;
using Timeline.Objects.Collection;
using Xamarin.Auth;

namespace Timeline.Models
{
    public class MUser
    {

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public CustomObservableCollection<MTimelineInfo> Timelines { get; set; }

        public MUser()
        {
            Timelines = new CustomObservableCollection<MTimelineInfo>();
            Clear();
        }

        public void Clear()
        {
            UserId = "";
            UserName = "";
            Email = "";
            PhotoUrl = "";
        }
        
        public MUser Copy()
        {
            MUser target = new MUser();
            target.UserId = UserId;
            target.UserName = UserName;
            target.Email = Email;
            target.PhotoUrl = PhotoUrl;
            foreach (MTimelineInfo tlinfo in Timelines) target.Timelines.Add(tlinfo);
            return target;
        }
    }
}
