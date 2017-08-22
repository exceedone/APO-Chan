using Prism.Unity;
using Apo_Chan.Views;
using Xamarin.Forms;

namespace Apo_Chan
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            MainPage = new Test.TestPage();
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            //NavigationService.NavigateAsync("NavigationPage/UserReportList");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<UserReportList>();
            Container.RegisterTypeForNavigation<NewReport>();
            Container.RegisterTypeForNavigation<DetailReport>();
        }

        private void ExecuteGoBack(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug] ExecuteGoBack 2");
            //NavigationService.GoBackAsync();
        }
    }
}
