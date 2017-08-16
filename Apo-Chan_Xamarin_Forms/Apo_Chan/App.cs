using Apo_Chan.Views;
using Prism.Unity;
using System;

using Xamarin.Forms;

namespace Apo_Chan
{
	public class App : PrismApplication
    {
		public App ()
		{
            // The root page of your application
            //MainPage = new Views.UserReportList();
        }

		protected override void OnInitialized()
        {
            NavigationService.NavigateAsync("NavigationPage/UserReportList");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<UserReportList>();
        }
    }
}

