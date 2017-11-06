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
    public class UserReportListViewModel : BaseViewModel, INavigatedAware
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

        private string reportHeaderLabel;
        /// <summary>
        /// Group Name, or "Your Report List"
        /// </summary>
        public string ReportHeaderLabel
        {
            get
            {
                if (this.reportHeaderLabel == null)
                {
                    return "Your Report List";
                }
                return $"Group Report List:{reportHeaderLabel}";
            }
            set
            {
                SetProperty(ref this.reportHeaderLabel, value);
            }
        }


        private bool isGroup;
        /// <summary>
        /// Is Group
        /// </summary>
        public bool IsGroup
        {
            get
            {
                return isGroup;
            }
            private set
            {
                SetProperty(ref this.isGroup, value);
            }
        }

        /// <summary>
        /// Select if user select grouo, set Group Id.
        /// </summary>
        public string TargetGroupId { get; set; }

        public DelegateCommand AddNewReportCommand { get; private set; }

        public DelegateCommand SelectGroupCommand { get; private set; }

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
            AddNewReportCommand = new DelegateCommand(NavigateNewReport);
            SelectGroupCommand = new DelegateCommand(NavigateSelectGroup);
            ItemTappedCommand = new DelegateCommand<ReportItem>(NavigateDetailReport);
            NextMonthReportCommand = new DelegateCommand(nextMonthReport);
            PrevMonthReportCommand = new DelegateCommand(prevMonthReport);

            GlobalAttributes.ShouldUpdateReports = true;
        }
        #endregion

        #region Function
        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            GlobalAttributes.ShouldUpdateReports = false;
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            //if (GlobalAttributes.ShouldUpdateReports)
            //{
            //    RefreshCommand.Execute();
            //}
            if (parameters.ContainsKey("GroupId") && parameters.ContainsKey("GroupName"))
            {
                this.IsGroup = true;
                this.TargetGroupId = (string)parameters["GroupId"];
                this.ReportHeaderLabel = Flurl.Url.DecodeQueryParamValue((string)parameters["GroupName"]);
            }
            else
            {
                this.IsGroup = false;
                this.TargetGroupId = App.SessionRepository.GetValue<string>(nameof(TargetGroupId));
                this.ReportHeaderLabel = App.SessionRepository.GetValue<string>(nameof(ReportHeaderLabel));
            }

            // Set Session
            App.SessionRepository.SetValue(nameof(TargetGroupId), TargetGroupId);
            App.SessionRepository.SetValue(nameof(ReportHeaderLabel), reportHeaderLabel);

            // navigate mode == new, set items
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                SetItemsAsync();
            }
        }

        public async void SetItemsAsync()
        {
            await setItemsAsync();
        }

        public async void NavigateNewReport()
        {
            await navigationService.NavigateAsync($"NewReport?GroupId={TargetGroupId}&GroupName={Flurl.Url.EncodeQueryParamValue(this.reportHeaderLabel, false)}");
        }

        public async void NavigateDetailReport(ReportItem item)
        {
            await navigationService.NavigateAsync($"DetailReport?Id={item.Id}&GroupId={TargetGroupId}&GroupName={Flurl.Url.EncodeQueryParamValue(this.reportHeaderLabel, false)}");
        }

        public async void NavigateSelectGroup()
        {
            await navigationService.NavigateAsync("GroupList?CalledType=1");
        }

        private async Task setItemsAsync()
        {
            //if (!GlobalAttributes.isConnectedInternet)
            //{
            //    await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
            //    return;
            //}

            IsBusy = true;
            ReportItems.Clear();
            ObservableCollection<ReportItem> allReports = null;
            try
            {
                // if select groupid, get reports referensed group
                if (!string.IsNullOrWhiteSpace(TargetGroupId))
                {
                    allReports = await CustomFunction.Get<ObservableCollection<ReportItem>>($"api/values/reportsbygroup/{TargetGroupId}/{this.CurrentDate.Year}/{this.CurrentDate.Month}");
                }
                // default
                else
                {
                    allReports = await ReportManager.DefaultManager.GetItemsAsync
                        (
                            this.CurrentDate.Year,
                            this.CurrentDate.Month
                        );
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
            }
            if (allReports != null)
            {
                foreach (var item in allReports)
                {
                    ReportItems.Add(item);
                }
            }

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
