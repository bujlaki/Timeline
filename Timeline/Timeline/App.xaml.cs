using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Timeline.ViewModels.Base;
using Amazon;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Timeline
{
	public partial class App : Application
	{
        public static readonly Timeline.Services.Base.ServiceContainer services = new Timeline.Services.Base.ServiceContainer();

		public App ()
		{
			InitializeComponent();

			var loggingConfig = AWSConfigs.LoggingConfig;
            loggingConfig.LogMetrics = true;
            loggingConfig.LogResponses = ResponseLoggingOption.Always;
            loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;
            loggingConfig.LogTo = LoggingOptions.SystemDiagnostics;
            
			AWSConfigs.AWSRegion = RegionEndpoint.EUCentral1.SystemName;
			AWSConfigs.CorrectForClockSkew = true;
            AWSConfigs.UseSdkCache = false;

            VMLocator locator = (VMLocator)Current.Resources["vmLocator"];

            //INITIALIZE SERVICES
            services.Set(new Services.NavigationService(locator));
            services.Set(new Services.DBService());
            services.Set(new Services.AuthenticationService());

            if (!DesignMode.IsDesignModeEnabled)
            {
                Task.Run(async ()=> await services.Authentication.GetCachedCredentials()).Wait();

                if (services.Authentication.Login.Type != Objects.Auth.LoginType.None) //if logged in
                {
                    MainPage = new NavigationPage(services.Navigation.UserPagesView());
                    locator.UserPagesViewModel.User = Task.Run(async () => await App.services.Database.GetUser(App.services.Authentication.Login.UserId)).Result;
                }
                else
                {
                    MainPage = new NavigationPage(services.Navigation.LoginPage());
                }
            }
            else
            {
                MainPage = new NavigationPage(services.Navigation.LoginPage());
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
