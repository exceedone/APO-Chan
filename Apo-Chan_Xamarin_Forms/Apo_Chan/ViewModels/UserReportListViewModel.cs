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

        public DateTime CurrentDate { get; set; }

        public bool IsBusy { get; set; }
        //public FileImageSource AddButtonImage { get; set; }

        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand<ReportItem> ItemTappedCommand { get; private set; }

        public DelegateCommand NavigateNewReportCommand { get; private set; }

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

        //constructor
        public UserReportListViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            ReportItems = new ObservableCollection<ReportItem>();
            CurrentDate = DateTime.Now;
            RefreshCommand = new DelegateCommand(SetItemsAsync);
            NavigateNewReportCommand = new DelegateCommand(NavigateNewReport);
            ItemTappedCommand = new DelegateCommand<ReportItem>(NavigateDetailReport);

            IsBusy = false;
            //AddButtonImage = (FileImageSource)ImageSource.FromFile("button_add_A.png");
        }

        public async void SetItemsAsync()
        {
            IsBusy = true;
            RaisePropertyChanged("IsBusy");

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
                    //GlobalAttributes.refUserId = allReports[0].RefUserId;
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
            RaisePropertyChanged("IsBusy");
        }

        private async void NavigateNewReport()
        {
            //AddButtonImage = (FileImageSource)ImageSource.FromFile("button_add_B.png");
            //RaisePropertyChanged("AddButtonImage");

            await navigationService.NavigateAsync("NewReport");

            //AddButtonImage = (FileImageSource)ImageSource.FromFile("button_add_A.png");
            //RaisePropertyChanged("AddButtonImage");
        }

        private void NavigateDetailReport(ReportItem item)
        {
            navigationService.NavigateAsync("DetailReport?Id=" + item.Id);
        }
    }
}
