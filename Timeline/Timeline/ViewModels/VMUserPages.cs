using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;

using Timeline.Models;
using Timeline.Objects.Timeline;

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
        private UserPagesMenuItem _selectedMenuItem;
        private MTimelineInfo _selectedTimeline;
        private MUser _user;
        private TimelineTheme theme;

        public ObservableCollection<MTimelineInfo> Timelines;
        public MTimelineInfo SelectedTimeline {
            get { return _selectedTimeline; }
            set
            {
                _selectedTimeline = value;
                RaisePropertyChanged("SelectedTimeline");
                if (_selectedTimeline == null) return;
                CmdTimelineSelected.Execute(_selectedTimeline);
                SelectedTimeline = null;
            }
        }

        public Command CmdMenu { get; set; }
        public Command CmdNewTimeline { get; set; }
        public Command CmdTimelineSelected { get; set; }
        public MUser User {
            get { return _user; }
            set { _user = value; RaisePropertyChanged("User"); }
        }        
        public ObservableCollection<UserPagesMenuItem> MenuItems { get; set; }
        public UserPagesMenuItem SelectedMenuItem
        {
            get { return _selectedMenuItem; }
            set
            {
                _selectedMenuItem = value;
                RaisePropertyChanged("SelectedItem");
                if (_selectedMenuItem == null) return;
                HandleMenuItem(_selectedMenuItem.Id);
                SelectedMenuItem = null;
            }
        }

        public VMUserPages() : base()
        {
            MenuItems = new ObservableCollection<UserPagesMenuItem>(new[]
            {
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Timelines, "My Timelines"),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Options, "Options" ),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.SignOut, "Sign out" ),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Test, "Test"),
            });

            CmdMenu = new Command(CmdMenuExecute);
            CmdNewTimeline = new Command(CmdNewTimelineExecute);
            CmdTimelineSelected = new Command(CmdTimelineSelectedExecute);

            //subscribe to events
            MessagingCenter.Subscribe<VMTimelineInfo, MTimelineInfo>(this, "TimelineInfo_created", TimelineInfo_created);
        }

        private void TimelineInfo_created(VMTimelineInfo arg1, MTimelineInfo arg2)
        {
            User.Timelines.Add(arg2);
            App.services.Database.UpdateUser(User);
        }

        public void CmdMenuExecute(object obj)
        {
            Views.VUserPages mainPage = (App.services.Navigation.UserPagesView() as Views.VUserPages);
            mainPage.IsPresented = true;
        }

        public void CmdNewTimelineExecute(object obj)
        {
            App.services.Navigation.GoToTimelineInfoView();
        }

        public void CmdTimelineSelectedExecute(object obj)
        {
            App.services.Navigation.GoToTimelineView((MTimelineInfo)obj);
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
                case UserPagesMenuItem.MenuItemID.Test:
                    App.services.Navigation.GoToTestPage();
                    break;
            }
            mainPage.IsPresented = false;
        }
    }
}
