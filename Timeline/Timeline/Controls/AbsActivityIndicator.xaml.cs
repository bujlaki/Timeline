using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Timeline.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AbsActivityIndicator : ContentView
	{
        #region "Bindable properties"
        public static readonly BindableProperty IsShowingProperty = BindableProperty.Create(
            nameof(IsShowing),
            typeof(bool),
            typeof(AbsActivityIndicator),
            false, BindingMode.OneWay,
            propertyChanged: OnIsShowingChanged);
        private static void OnIsShowingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            AbsActivityIndicator abs = ((AbsActivityIndicator)bindable);
            abs.indicator.IsRunning = (bool)newValue;
            abs.InvalidateLayout();
        }
        public bool IsShowing
        {
            get { return (bool)GetValue(IsShowingProperty); }
            set { SetValue(IsShowingProperty, value); }
        }

        public static readonly BindableProperty MessageProperty = BindableProperty.Create(
            nameof(Message),
            typeof(string),
            typeof(AbsActivityIndicator),
            "", BindingMode.OneWay,
            propertyChanged: OnMessageChanged);
        private static void OnMessageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            AbsActivityIndicator abs = ((AbsActivityIndicator)bindable);
            abs.message.Text = (string)newValue;
            abs.InvalidateLayout();
        }
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        #endregion

        public AbsActivityIndicator ()
		{
			InitializeComponent ();
		}
	}
}