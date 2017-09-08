using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class UserReportListViewModel : BaseViewModel
    {
        #region Variable and Property
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

        private DateTime currentDate;
        public DateTime CurrentDate
        {
            get
            {
                return currentDate;
            }
            set
            {
                SetProperty(ref this.currentDate, value);
            }
        }

        public DelegateCommand RefreshCommand { get; private set; }

        public DelegateCommand AddNewReportCommand { get; private set; }

        public DelegateCommand<ReportItem> ItemTappedCommand { get; private set; }

        public DelegateCommand NextMonthReportCommand { get; private set; }

        public DelegateCommand PrevMonthReportCommand { get; private set; }

        #endregion

        #region Constructor
        //constructor
        public UserReportListViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            ReportItems = new ObservableCollection<ReportItem>();
            CurrentDate = DateTime.Now;
            RefreshCommand = new DelegateCommand(SetItemsAsync).ObservesProperty(() => IsBusy);
            AddNewReportCommand = new DelegateCommand(NavigateNewReport);
            ItemTappedCommand = new DelegateCommand<ReportItem>(NavigateDetailReport);
            NextMonthReportCommand = new DelegateCommand(nextMonthReport);
            PrevMonthReportCommand = new DelegateCommand(prevMonthReport);
        }
        #endregion

        #region Function
        public async void SetItemsAsync()
        {
            await setItemsAsync();
        }

        public async void NavigateNewReport()
        {
            await navigationService.NavigateAsync("NewReport");
        }

        public void NavigateDetailReport(ReportItem item)
        {
            navigationService.NavigateAsync("DetailReport?Id=" + item.Id);
        }

        private async Task setItemsAsync()
        {
            IsBusy = true;

            ObservableCollection<ReportItem> allReports = null;
            try
            {
                allReports = await ReportManager.DefaultManager.GetItemsAsync
                    (
                        this.CurrentDate.Year,
                        this.CurrentDate.Month
                    );
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
            }

            ReportItems.Clear();
            ReportItems = allReports;

            IsBusy = false;
        }

        private async void nextMonthReport()
        {
            this.CurrentDate = this.CurrentDate.AddMonths(1);
            await this.setItemsAsync();
        }
        private async void prevMonthReport()
        {
            this.CurrentDate = this.CurrentDate.AddMonths(-1);
            await this.setItemsAsync();
        }
        #endregion

    }
}
