using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Acr.UserDialogs;
using Xamarin.Essentials;

using Timeline.Models;
using Timeline.Objects.Timeline;
using System.IO;
using System.Reflection;

namespace Timeline.ViewModels
{
    public class UserPagesMenuItem
    {
        public enum MenuItemID
        {
            Timelines,
            Options,
            SignOut,
            Test
        }

        public MenuItemID Id { get; set; }
        public string Title { get; set; }

        public UserPagesMenuItem(MenuItemID _id, string _title)
        {
            Id = _id;
            Title = _title;
        }
    }

    public class VMUserPages : Base.VMBase
    {
        //private MTimelineInfo selectedTimeline;
        //private TimelineTheme theme;
        public string SearchText { get; set; }

        public ObservableCollection<UserPagesMenuItem> MenuItems { get; set; }

        private UserPagesMenuItem selectedMenuItem;
        public UserPagesMenuItem SelectedMenuItem
        {
            get { return selectedMenuItem; }
            set
            {
                selectedMenuItem = value;
                RaisePropertyChanged("SelectedMenuItem");
                if (selectedMenuItem == null) return;
                HandleMenuItem(selectedMenuItem.Id);
                SelectedMenuItem = null;
            }
        }

        private MUser _user;
        public MUser User
        {
            get { return _user; }
            set { _user = value; RaisePropertyChanged("User"); }
        }

        //public ObservableCollection<MTimelineInfo> Timelines;
        public ObservableCollection<MTimelineInfo> TimelineSearchResults { get; set; }
        public ObservableCollection<MTimelineInfo> TimelineFavorites { get; set; }

        //tab segments
        public int SelectedSegment { get; set; }
        public bool ShowSegment0 { get { return SelectedSegment == 0; } } 
        public bool ShowSegment1 { get { return SelectedSegment == 1; } }
        public bool ShowSegment2 { get { return SelectedSegment == 2; } }

        //commands
        public Command CmdMenu { get; set; }
        public Command CmdTabSegmentTap { get; set; }
        public Command CmdNewTimeline { get; set; }
        public Command CmdShowTimeline { get; set; }
        public Command CmdEditTimeline { get; set; }
        public Command CmdDeleteTimeline { get; set; }
        public Command CmdShareTimeline { get; set; }
        public Command CmdAddToFavorite { get; set; }
        public Command CmdSearch { get; set; }
        public Command CmdPrivacyPolicy { get; set; }
        public Command CmdDeleteAccount { get; set; }


        public VMUserPages() : base()
        {
            MenuItems = new ObservableCollection<UserPagesMenuItem>(new[]
            {
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Timelines, "My Timelines"),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Options, "Options" ),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.SignOut, "Sign out" ),
            });

            CmdMenu = new Command(CmdMenuExecute);
            CmdTabSegmentTap = new Command(CmdTabSegmentTapExecute);
            CmdNewTimeline = new Command(CmdNewTimelineExecute);
            CmdShowTimeline = new Command(CmdShowTimelineExecute);
            CmdEditTimeline = new Command(CmdEditTimelineExecute);
            CmdDeleteTimeline = new Command(CmdDeleteTimelineExecute);
            CmdShareTimeline = new Command(CmdShareTimelineExecute);
            CmdAddToFavorite = new Command(CmdAddToFavoriteExecute);
            CmdSearch = new Command(CmdSearchExecute);
            CmdPrivacyPolicy = new Command(CmdPrivacyPolicyExecute);
            CmdDeleteAccount = new Command(CmdDeleteAccountExecute);

            //subscribe to events
            MessagingCenter.Subscribe<VMTimelineInfo, MTimelineInfo>(this, "TimelineInfo_created", TimelineInfo_created);
            MessagingCenter.Subscribe<VMTimelineInfo, MTimelineInfo>(this, "TimelineInfo_updated", TimelineInfo_updated);
        }

        public void InitView(MUser loggedInUser)
        {
            User = loggedInUser;
            TimelineFavorites = new ObservableCollection<MTimelineInfo>(Task.Run(async () => await App.services.Database.GetSharedTimelinesByIDs(User.Favorites)).Result);

        }

        private void TimelineInfo_created(VMTimelineInfo arg1, MTimelineInfo arg2)
        {
            User.Timelines.Add(arg2);
            App.services.Database.UpdateUser(User);
        }

        private void TimelineInfo_updated(VMTimelineInfo arg1, MTimelineInfo arg2)
        {
            App.services.Database.UpdateUser(User);

            //if it is shared, need to update the shared record, and the tags
            if (arg2.Shared)
            {
                App.services.Database.UpdateSharedTimeline(arg2, User);
                App.services.Database.UpdateSharedTimelineTags(arg2);
            }

            User.Timelines.ReportItemChange(arg2);
        }

        public void CmdMenuExecute(object obj)
        {
            Views.VUserPages mainPage = (App.services.Navigation.UserPagesView() as Views.VUserPages);
            mainPage.IsPresented = true;
        }

        public void CmdTabSegmentTapExecute(object obj)
        {
            RaisePropertyChanged("ShowSegment0");
            RaisePropertyChanged("ShowSegment1");
            RaisePropertyChanged("ShowSegment2");
        }

        public void CmdNewTimelineExecute(object obj)
        {
            App.services.Navigation.GoToTimelineInfoView(null);
        }

        public void CmdShowTimelineExecute(object obj)
        {
            App.services.Navigation.GoToTimelineView((MTimelineInfo)obj);
        }

        public void CmdEditTimelineExecute(object obj)
        {
            App.services.Navigation.GoToTimelineInfoView((MTimelineInfo)obj);
        }

        public void CmdDeleteTimelineExecute(object obj)
        {
            Task.Run(async () =>
            {
                ConfirmConfig cc = new ConfirmConfig();
                cc.Message = "Are you sure?";
                cc.Title = "Delete timeline";
                if (await UserDialogs.Instance.ConfirmAsync(cc))
                {
                    MTimelineInfo tlinfo = (MTimelineInfo)obj;
                    User.Timelines.Remove(tlinfo);
                    await App.services.Database.UpdateUser(User);
                }
            });
        }

        public void CmdShareTimelineExecute(object obj)
        {
            MTimelineInfo tlinfo = (MTimelineInfo)obj;
            tlinfo.Shared = true;
            tlinfo.OwnerID = User.UserId;
            tlinfo.OwnerName = User.UserName;

            App.services.Database.ShareTimeline(tlinfo, User);
            App.services.Database.UpdateUser(User);
            User.Timelines.ReportItemChange(tlinfo);
        }

        public void CmdAddToFavoriteExecute(object obj)
        {
            MTimelineInfo tlinfo = (MTimelineInfo)obj;

            if(User.Favorites.Contains(tlinfo.TimelineId))
            {
                UserDialogs.Instance.Toast("This Timeline is already added to your favorites!");
                return;
            }

            User.Favorites.Add(tlinfo.TimelineId);
            TimelineFavorites.Add(tlinfo);
            App.services.Database.UpdateUser(User);
            RaisePropertyChanged("TimelineFavorites");
        }

        public async void CmdSearchExecute(object obj)
        {
            TimelineSearchResults = new ObservableCollection<MTimelineInfo>(await App.services.Database.SearchSharedTimeline(SearchText));
            RaisePropertyChanged("TimelineSearchResults");
        }

        void CmdPrivacyPolicyExecute(object obj)
        {
            byte[] buffer;
            string strpp;
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("Timeline.Embedded.Text.PrivacyPolicy.txt"))
            {
                long length = s.Length;
                buffer = new byte[length];
                s.Read(buffer, 0, (int)length);
                strpp = System.Text.Encoding.Default.GetString(buffer);
            }

            //string s = "this is an example text";
            //string msg = "";
            //for (int i = 0; i < 100; i++) msg = msg + s + " ";

            AlertConfig ac = new AlertConfig();
            ac.Title = "Privacy Policy";
            ac.Message = strpp;

            UserDialogs.Instance.Alert(ac);
        }

        void CmdDeleteAccountExecute(object obj)
        {
            Task.Run(async () =>
            {
                ConfirmConfig cc = new ConfirmConfig();
                cc.Title = "Delete user account";
                cc.Message = "Are you sure to delete your account? This means your user data and all Timelines (including shared ones) will be deleted along with their event data.";
                if (await UserDialogs.Instance.ConfirmAsync(cc))
                {
                    List<string> sharedTimelineIds = new List<string>();
                    foreach (MTimelineInfo tlinfo in User.Timelines)
                    {
                        //remove shared timeline tags
                        if (tlinfo.Shared)
                        {
                            await App.services.Database.DeleteSharedTimelineTags(tlinfo);
                            sharedTimelineIds.Add(tlinfo.TimelineId);
                        }

                        //delete timeline events
                        await App.services.Database.DeleteEventsByTimelineId(tlinfo.TimelineId);
                    }

                    //delete shared timelines
                    if (sharedTimelineIds.Count > 0)
                        await App.services.Database.DeleteSharedTimelinesByIDs(sharedTimelineIds);

                    //delete user from DB
                    await App.services.Database.DeleteUser(User);

                    //delete Cognito user
                    if(App.services.Authentication.Login.Type == Objects.Auth.LoginType.Cognito)
                    {
                        await App.services.Authentication.DeleteCognitoUser(User.UserName);
                    }
                }
            });
        }

        private void HandleMenuItem(UserPagesMenuItem.MenuItemID id)
        {
            Views.VUserPages mainPage = (App.services.Navigation.UserPagesView() as Views.VUserPages);
            switch (id)
            {
                case UserPagesMenuItem.MenuItemID.Timelines:
                    mainPage.Detail = new NavigationPage(App.services.Navigation.TimelineListView());
                    break;
                case UserPagesMenuItem.MenuItemID.Options:
                    mainPage.Detail = new NavigationPage(App.services.Navigation.OptionsView());
                    break;
                case UserPagesMenuItem.MenuItemID.SignOut:
                    App.services.Authentication.SignOut();
                    App.services.Navigation.GoToLoginPage();
                    break;
                //case UserPagesMenuItem.MenuItemID.Test:
                //    App.services.Navigation.GoToTestPage();
                //    break;
            }
            mainPage.IsPresented = false;
        }
    }
}
