using Prism.Unity;
using Apo_Chan.Views;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Apo_Chan.Models;

namespace Apo_Chan
{
    public partial class App : PrismApplication
    {
        private static MobileServiceClient client;
        public static IAuthenticate Authenticator { get; private set; }

        public App() : base(null)
        {
            //client = new MobileServiceClient(Constants.ApplicationURL);
        }
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            //client = new MobileServiceClient(Constants.ApplicationURL);
        }

        public static MobileServiceClient CurrentClient
        {
            get {
                if (client == null) { client = new MobileServiceClient(Constants.ApplicationURL); }
                return client;
            }
        }

        protected async override void OnInitialized()
        {
            InitializeComponent();

            //if (await BaseAuthProvider.FirstProcess())
            //{
            //    await NavigationService.NavigateAsync("NavigationPage/UserReportList");
            //}
            //else
            //{
            //    await NavigationService.NavigateAsync("NavigationPage/SignIn");
            //}
            await NavigationService.NavigateAsync("NavigationPage/SignIn");

            GlobalAttributes.Geolocator.DesiredAccuracy = 100;
            //GlobalAttributes.Position = await GlobalAttributes.Geolocator.GetPositionAsync(10000);
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<SignIn>();
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<Setting>();
            Container.RegisterTypeForNavigation<UserReportList>();
            Container.RegisterTypeForNavigation<NewReport>();
            Container.RegisterTypeForNavigation<DetailReport>();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }
    }
}
