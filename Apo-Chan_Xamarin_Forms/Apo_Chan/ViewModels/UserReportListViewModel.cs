using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : BindableBase
    {
        private INavigationService navigationService;

        private ObservableCollection<ReportItem> reportItems;
        public ObservableCollection<ReportItem> ReportItems
        {
            get
            {
                return reportItems;
            }
            set
            {
                SetProperty(ref this.reportItems, value);
            }
        }

        public DateTime CurrentDate { get; set; }

        private bool isBusy = false;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                SetProperty(ref this.isBusy, value);
            }
        }

        private FileImageSource addButtonImage = (FileImageSource)ImageSource.FromFile("button_add_A.png");
        public FileImageSource AddButtonImage
        {
            get
            {
                return addButtonImage;
            }
            set
            {
                SetProperty(ref this.addButtonImage, value);
            }
        }

        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand AddNewReportCommand { get; private set; }

        public DelegateCommand<ReportItem> ItemTappedCommand { get; private set; }

        //constructor
        public UserReportListViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            ReportItems = new ObservableCollection<ReportItem>();
            CurrentDate = DateTime.Now;
            RefreshCommand = new DelegateCommand(SetItemsAsync).ObservesProperty(() => IsBusy);
            AddNewReportCommand = new DelegateCommand(NavigateNewReport).ObservesProperty(() => AddButtonImage); ;
            ItemTappedCommand = new DelegateCommand<ReportItem>(NavigateDetailReport);
        }

        public async void SetItemsAsync()
        {
            IsBusy = true;

            ObservableCollection<ReportItem> allReports = null;
            try
            {
                allReports = await ReportManager.DefaultManager.GetItemsAsync();
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
            }

            if (allReports != null)
            {
                if (allReports.Count > 0)
                {
                    GlobalAttributes.refUserId = allReports[0].RefUserId;
                }
                ReportItems.Clear();
                foreach (var item in allReports)
                {
                    if (!item.Deleted)
                    {
                        ReportItems.Add(item);
                    }
                }
            }

            IsBusy = false;
        }

        private async void NavigateNewReport()
        {
            AddButtonImage = (FileImageSource)ImageSource.FromFile("button_add_B.png");

            await navigationService.NavigateAsync("NewReport");

            AddButtonImage = (FileImageSource)ImageSource.FromFile("button_add_A.png");
        }

        private void NavigateDetailReport(ReportItem item)
        {
            navigationService.NavigateAsync("DetailReport?Id=" + item.Id);
        }
    }
}
