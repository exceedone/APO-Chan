using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : BaseViewModel
    {
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

        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand AddNewReportCommand { get; private set; }

        public DelegateCommand<ReportItem> ItemTappedCommand { get; private set; }

        //constructor
        public UserReportListViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            ReportItems = new ObservableCollection<ReportItem>();
            CurrentDate = DateTime.Now;
            RefreshCommand = new DelegateCommand(SetItemsAsync).ObservesProperty(() => IsBusy);
            AddNewReportCommand = new DelegateCommand(NavigateNewReport);
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
        }

        private async void NavigateNewReport()
        {
            await navigationService.NavigateAsync("NewReport");
        }

        private void NavigateDetailReport(ReportItem item)
        {
            navigationService.NavigateAsync("DetailReport?Id=" + item.Id);
        }
    }
}
