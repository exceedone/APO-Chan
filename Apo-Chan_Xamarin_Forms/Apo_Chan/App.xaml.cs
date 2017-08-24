using Prism.Unity;
using Apo_Chan.Views;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;

using System.Threading.Tasks;

namespace Apo_Chan
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync(Constants.EProviderType providerType);
        Task SignOutAsync();
    }

    public partial class App : PrismApplication
    {
        static MobileServiceClient client;
        public static IAuthenticate Authenticator { get; private set; }

        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
            client = new MobileServiceClient(Constants.ApplicationURL);
            //MainPage = new Test.TestPage();
            MainPage = new Test.TestPage();
        }

        public static MobileServiceClient CurrentClient
        {
            get { return client; }
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            NavigationService.NavigateAsync("NavigationPage/SignIn");
            //NavigationService.NavigateAsync("NavigationPage/UserReportList");
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<SignIn>();
            Container.RegisterTypeForNavigation<NavigationPage>();
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
