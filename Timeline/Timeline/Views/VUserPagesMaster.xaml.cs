using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Timeline.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VUserPagesMaster : ContentPage
    {
        //public ListView ListView;

        public VUserPagesMaster()
        {
            InitializeComponent();

            //BindingContext = new VUserPagesMasterViewModel();
            //ListView = MenuItemsListView;
        }

        //class VUserPagesMasterViewModel : INotifyPropertyChanged
        //{
        //    public ObservableCollection<VUserPagesMenuItem> MenuItems { get; set; }
            
        //    public VUserPagesMasterViewModel()
        //    {
        //        MenuItems = new ObservableCollection<VUserPagesMenuItem>(new[]
        //        {
        //            new VUserPagesMenuItem { Id = 0, Title = "Page 1" },
        //            new VUserPagesMenuItem { Id = 1, Title = "Page 2" },
        //            new VUserPagesMenuItem { Id = 2, Title = "Page 3" },
        //            new VUserPagesMenuItem { Id = 3, Title = "Page 4" },
        //            new VUserPagesMenuItem { Id = 4, Title = "Page 5" },
        //        });
        //    }
            
        //    #region INotifyPropertyChanged Implementation
        //    public event PropertyChangedEventHandler PropertyChanged;
        //    void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //    {
        //        if (PropertyChanged == null)
        //            return;

        //        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //    #endregion
        //}
    }
}