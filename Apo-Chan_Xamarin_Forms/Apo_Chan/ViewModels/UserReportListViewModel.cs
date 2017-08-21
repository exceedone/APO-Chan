using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : BindableBase
    {
        private INavigationService navigationService;

        public DateTime CurrentDate { get; set; }

        public bool IsBusy { get; set; }

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
            IsBusy = false;
            ItemTappedCommand = new DelegateCommand<ReportItem>(NavigateDetailReport);
        }

        public async void SetItemsAsync()
        {
            IsBusy = true;
            RaisePropertyChanged("IsBusy");

            ObservableCollection<ReportItem> allReports = new ObservableCollection<ReportItem>();
            try
            {
                allReports = await ReportManager.DefaultManager.GetItemsAsync();
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
            }

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

            IsBusy = false;
            RaisePropertyChanged("IsBusy");
        }

        private void NavigateNewReport()
        {
            navigationService.NavigateAsync("NewReport");
        }

        private void NavigateDetailReport(ReportItem item)
        {
            navigationService.NavigateAsync("DetailReport?Id=" + item.Id);
        }
    }
}
