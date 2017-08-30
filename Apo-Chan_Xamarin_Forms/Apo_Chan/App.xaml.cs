using Prism.Unity;
using Apo_Chan.Views;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Apo_Chan.Models;
using System.Threading.Tasks;

namespace Apo_Chan
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync(Constants.EProviderType providerType);
    }

    public partial class App : PrismApplication
    {
        private static MobileServiceClient client;
        public static IAuthenticate Authenticator { get; private set; }

        public App() : base(null)
        {
            //client = new MobileServiceClient(Constants.ApplicationURL);
            //MainPage = new Test.TestPage();
        }
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            //client = new MobileServiceClient(Constants.ApplicationURL);
            //MainPage = new Test.TestPage();
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

        private void ExecuteGoBack(object sender, System.EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug] ExecuteGoBack 2");
            //NavigationService.GoBackAsync();
        }
    }
}
