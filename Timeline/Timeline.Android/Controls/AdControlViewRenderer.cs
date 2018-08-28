using Android.Widget;
using Android.Gms.Ads;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Plugin.CurrentActivity;

[assembly: ExportRenderer(typeof(Timeline.Controls.AdControlView), typeof(Timeline.Droid.Controls.AdControlViewRenderer))]
namespace Timeline.Droid.Controls
{
    public class AdControlViewRenderer : ViewRenderer<Timeline.Controls.AdControlView, AdView>
    {
        string adUnitId = string.Empty;
        //Note you may want to adjust this, see further down.
        readonly AdSize adSize = AdSize.SmartBanner;
        AdView adView;

        public AdControlViewRenderer(Context context) : base(context)
        {
        }

        private AdView CreateNativeAdControl()
        {
            if (adView != null)
            {
                return adView;
            }

            // This is a string in the Resources/values/strings.xml that I added or you can modify it here. This comes from admob and contains a / in it
            //adUnitId = "ca-app-pub-5812987721297534/3564925203"; //Forms.Context.Resources.GetString(Resource.String.banner_ad_unit_id);
            //TEST AD UNIT
            adUnitId = "ca-app-pub-3940256099942544/6300978111";

            adView = new AdView(CrossCurrentActivity.Current.AppContext); //new AdView(Forms.Context);
            adView.AdSize = adSize;
            adView.AdUnitId = adUnitId;

            var adParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            adView.LayoutParameters = adParams;
            
            adView.AdListener = new MyAdListener();

            adView.LoadAd(new AdRequest
                            .Builder()
                            .Build());
            return adView;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Timeline.Controls.AdControlView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                CreateNativeAdControl();
                SetNativeControl(adView);
            }
        }

        class MyAdListener : AdListener
        {
            public MyAdListener()
            {
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                base.OnAdFailedToLoad(errorCode);
                Android.Util.Log.Info("MyAdListener", "Error: " + errorCode.ToString());
            }
        }
    }
}