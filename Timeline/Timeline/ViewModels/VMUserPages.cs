using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using Xamarin.Forms;

using Timeline.Models;

namespace Timeline.ViewModels
{
    public class UserPagesMenuItem
    {
        public enum MenuItemID
        {
            Timelines,
            Options,
            SignOut
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
        private UserPagesMenuItem _selectedItem;

        public Command CmdMenu { get; set; }
        public MUser LoggedInUser { get { return _services.Authentication.CurrentUser; }  }        

        public ObservableCollection<UserPagesMenuItem> MenuItems { get; set; }
        public UserPagesMenuItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");
                if (_selectedItem == null)
                    return;
                HandleMenuItem(_selectedItem.Id);
                SelectedItem = null;
            }
        }

        public VMUserPages(Services.Base.ServiceContainer services) : base(services)
        {
            MenuItems = new ObservableCollection<UserPagesMenuItem>(new[]
            {
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Timelines, "My Timelines"),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.Options, "Options" ),
                    new UserPagesMenuItem (UserPagesMenuItem.MenuItemID.SignOut, "Sign out" ),
            });

            CmdMenu = new Command(CmdMenuExecute);
            //LoggedInUser = _services.Authentication.CurrentUser;
            //SetDetailPage(UserPagesMenuItem.MenuItemID.Timelines);
        }

        public void CmdMenuExecute(object obj)
        {
            Views.VUserPages mainPage = (_services.Navigation.UserPagesView() as Views.VUserPages);
            mainPage.IsPresented = true;
        }

        private void HandleMenuItem(UserPagesMenuItem.MenuItemID id)
        {
            Views.VUserPages mainPage = (_services.Navigation.UserPagesView() as Views.VUserPages);
            switch (id)
            {
                case UserPagesMenuItem.MenuItemID.Timelines:
                    mainPage.Detail = new NavigationPage(_services.Navigation.TimelineListView());
                    break;
                case UserPagesMenuItem.MenuItemID.Options:
                    mainPage.Detail = new NavigationPage(_services.Navigation.OptionsView());
                    break;
                case UserPagesMenuItem.MenuItemID.SignOut:
                    _services.Authentication.CurrentUser.Clear();
                    _services.Authentication.ClearCachedCredentials();
                    App.Current.MainPage = new NavigationPage(_services.Navigation.RootPage());
                    break;
            }
            mainPage.IsPresented = false;
        }
    }
}
