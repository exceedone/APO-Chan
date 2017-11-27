using System;
using Prism.Unity;
using Apo_Chan.Views;
using Apo_Chan.Service;
using Xamarin.Forms;
using Microsoft.WindowsAzure.MobileServices;
using Apo_Chan.Models;

namespace Apo_Chan
{
    public partial class App : PrismApplication
    {
        private static MobileServiceClient client;
        public static ISessionRepository SessionRepository;
        public static IAuthenticate Authenticator { get; private set; }

        static App()
        {
            SessionRepository = new ApplicationProperties();
            SessionRepository.Initilize();
        }

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

            if (BaseAuthProvider.FirstProcess())
            {
                await NavigationService.NavigateAsync("NavigationPage/UserReportList");
            }
            else
            {
                await NavigationService.NavigateAsync("NavigationPage/SignIn");
            }

            Geolocation.GeoService.Init();
            OfflineSync.InitOfflineSyncContext();
        }

        protected override void RegisterTypes()
        {
            Container.RegisterTypeForNavigation<SignIn>();
            Container.RegisterTypeForNavigation<NavigationPage>();
            Container.RegisterTypeForNavigation<Setting>();
            Container.RegisterTypeForNavigation<UserReportList>();
            Container.RegisterTypeForNavigation<NewReport>();
            Container.RegisterTypeForNavigation<DetailReport>();
            Container.RegisterTypeForNavigation<GroupList>();
            Container.RegisterTypeForNavigation<NewGroup>();
            Container.RegisterTypeForNavigation<DetailGroup>();
        }

        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        //
        // 概要:
        //     App developers override this method to respond when the user initiates an app
        //     link request.
        //
        // パラメーター:
        //   uri:
        //     The URI for the request.
        //
        // コメント:
        //     To be added.
        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);
        }
    }
}
