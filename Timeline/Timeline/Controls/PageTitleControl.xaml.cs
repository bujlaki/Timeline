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
	public partial class PageTitleControl : ContentView
	{
        #region "Bindable properties"
        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(PageTitleControl),
            "", BindingMode.OneWay,
            propertyChanged: OnTitleChanged);
        private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PageTitleControl abs = ((PageTitleControl)bindable);
            abs.titleLabel.Text = (string)newValue;
            abs.InvalidateLayout();
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
            nameof(TitleColor),
            typeof(Color),
            typeof(PageTitleControl),
            Color.FromHex("#eeebd3"), BindingMode.OneWay,
            propertyChanged: OnTitleColorChanged);
        private static void OnTitleColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PageTitleControl abs = ((PageTitleControl)bindable);
            abs.titleLabel.TextColor = (Color)newValue;
            abs.InvalidateLayout();
        }
        public Color TitleColor
        {
            get { return (Color)GetValue(TitleColorProperty); }
            set { SetValue(TitleColorProperty, value); }
        }

        public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
            nameof(LineColor),
            typeof(Color),
            typeof(PageTitleControl),
            Color.FromHex("#f4bd4f"), BindingMode.OneWay,
            propertyChanged: OnLineColorChanged);
        private static void OnLineColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            PageTitleControl abs = ((PageTitleControl)bindable);
            abs.titleLine.Color = (Color)newValue;
            abs.InvalidateLayout();
        }
        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }
        #endregion

        public PageTitleControl ()
		{
			InitializeComponent ();
		}
	}
}