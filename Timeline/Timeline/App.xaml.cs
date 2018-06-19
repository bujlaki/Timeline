﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Timeline.ViewModels.Base;
using Amazon;

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

            //CHECK CACHED COGNITO IDENTITY
            if (!DesignMode.IsDesignModeEnabled)
            {
                if (((VMLocator)Current.Resources["vmLocator"]).Services.Authentication.GetCachedCredentials())
                {
                    MainPage = new NavigationPage(
                        ((VMLocator)Current.Resources["vmLocator"]).Services.Navigation.UserPagesView()
                        );
                }
                else
                {
                    MainPage = new NavigationPage(
                        ((VMLocator)Current.Resources["vmLocator"]).Services.Navigation.RootPage()
                        );
                }
            }
            else
            {
                MainPage = new NavigationPage(
                    ((VMLocator)Current.Resources["vmLocator"]).Services.Navigation.RootPage()
                    );
            }

            //MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
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
