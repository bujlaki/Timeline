using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Timeline.ViewModels.Base;
using Amazon;
using System.Threading.Tasks;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Timeline
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

			var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
            loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;
            
			AWSConfigs.AWSRegion = AwsRegion.EUCentral1.Name;
			AWSConfigs.CorrectForClockSkew = true;

            VMLocator locator = (VMLocator)Current.Resources["vmLocator"];

            if (!DesignMode.IsDesignModeEnabled)
            {
                Task.Run(async ()=> await locator.Services.Authentication.GetCachedCredentials()).Wait();

                if (locator.Services.Authentication.CurrentUser.LoggedIn)
                {
                    MainPage = new NavigationPage(locator.Services.Navigation.UserPagesView());
                    locator.UserPagesViewModel.User = locator.Services.Authentication.CurrentUser;
                }
                else
                {
                    MainPage = new NavigationPage(locator.Services.Navigation.LoginPage());
                }
            }
            else
            {
                MainPage = new NavigationPage(locator.Services.Navigation.LoginPage());
            }
        }

		protected override void OnStart ()
		{
            // Handle when your app starts
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
