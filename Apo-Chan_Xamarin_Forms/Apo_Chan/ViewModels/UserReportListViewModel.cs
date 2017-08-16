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
        public DelegateCommand NavigateNewReportCommand { get; private set; }

        public DateTime CurrentDate { get; set; }

        public DelegateCommand RefreshCommand { get; private set; }

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
        }

        public async void SetItemsAsync()
        {
            this.ReportItems = await ReportManager.DefaultManager.GetItemsAsync();
        }

        private void NavigateNewReport()
        {
            navigationService.NavigateAsync("NewReport");
        }
    }
}
