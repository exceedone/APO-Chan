using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class DetailReportViewModel : BaseReportViewModel
    {
        #region Variable and Property
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

        private bool isEdit;
        public bool IsEdit
        {
            get
            {
                return this.isEdit;
            }
            set
            {
                SetProperty(ref this.isEdit, value);
            }
        }
        #endregion

        #region Constructor
        public DetailReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            this.IsEdit = true;
            UpdateCommand = new DelegateCommand(updateReport);
            DeleteCommand = new DelegateCommand(deleteReport);
        }
        #endregion

        #region Function
        private async void updateReport()
        {
            if (isValidReport())
            {
                var accepted = await dialogService.DisplayAlertAsync
                    (
                        "Confirmation",
                        "Do you want to update the report?",
                        "Confirm",
                        "Cancel"
                    );
                if (accepted)
                {
                    IsBusy = true;
                    Report.PropertyChanged -= OnDateTimeChanged;
                    try
                    {
                        await ReportManager.DefaultManager.SaveTaskAsync(Report);

                        // has groupids, post 
                        //if (!string.IsNullOrWhiteSpace(this.groupIds))
                        //{
                        //    await CustomFunction.Post($"table/reportgroup/list/{this.Report.Id}", this.groupIds.Split(',').Select(x => new ReportGroupItem()
                        //    {
                        //        RefGroupId = x
                        //            ,
                        //        RefReportId = Report.Id
                        //    }));
                        //}

                        //In the case remove groups from report, this method will delete ReportGroup entry
                        await ReportGroupManager.DefaultManager.UpsertReport(this.Report.Id, this.groupIds);
                    }
                    catch (Exception e)
                    {
                        Models.DebugUtil.WriteLine("DetailReportViewModel > " + e.Message);
                    }
                    IsBusy = false;
                    //await this.navigateTop();
                    NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Reset", true);
                await navigationService.GoBackAsync(parameters);
                }
            }
        }

        private async void deleteReport()
        {
            var accepted = await dialogService.DisplayAlertAsync
                (
                    "Confirmation",
                    "Do you want to delete the report?",
                    "Confirm",
                    "Cancel"
                );
            if (accepted)
            {
                IsBusy = true;
                try
                {
                    //### delete relational records of ReportGroup then delete the report
                    await ReportGroupManager.DefaultManager.DeleteReportAsync(Report);
                    await ReportManager.DefaultManager.DeleteAsync(Report);
                }
                catch (Exception e)
                {
                    Models.DebugUtil.WriteLine("DetailReportViewModel > " + e.Message);
                }
                IsBusy = false;
                NavigationParameters parameters = new NavigationParameters();
                parameters.Add("Reset", true);
                await navigationService.GoBackAsync(parameters);
            }
        }

        protected override void selectGroup()
        {
            if (!isEdit)
            {
                return;
            }
            base.selectGroup();
        }

        public async override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("Id"))
                {
                    IsBusy = true;
                    try
                    {
                        Report = await ReportManager.DefaultManager.LookupAsync((string)parameters["Id"]);
                        this.IsEdit = report.RefUserId == GlobalAttributes.User.Id;

                        if (Report.ReportLat != 0 && Report.ReportLon != 0)
                        {
                            GpsImage = "ic_gps_on.png";
                        }
                        // get reportGroup
                        //var reportGroupItems = await CustomFunction.Get<List<GroupItem>>($"api/values/groupsbyreport/{Report.Id}");
                        var reportGroupItems = await ReportGroupManager.DefaultManager.GetGroupsByReport(Report.Id);
                        if (reportGroupItems.Any())
                        {
                            this.groupIds = string.Join(",", reportGroupItems.Select(x => x.Id));
                            this.GroupLabel = string.Join(",", reportGroupItems.Select(x => x.GroupName));
                        }
                    }
                    catch (Exception e)
                    {

                        Models.DebugUtil.WriteLine("DetailReportViewModel > " + e.Message);
                    }
                    IsBusy = false;
                    if (Report != null)
                    {
                        UpdateLocationCommand.Execute();
                        Report.PropertyChanged += OnDateTimeChanged;
                    }
                }
                else
                {
                    await dialogService.DisplayAlertAsync("Error", "Failed to load the detail page!", "OK");
                    await navigationService.GoBackAsync();
                }
            }
        }

        protected override void updateLocation()
        {
            if (!IsEdit)
            {
                return;
            }
            if (Report != null)
            {
                if (Report.ReportLat == 0 && Report.ReportLon == 0)
                {
                    GpsImage = "ic_gps_off.png";
                    LocationText = "No location information.";
                    LocationTextColor = (Color)App.Current.Resources["SecondaryTextColor"];
                    return;
                }
                if (Report.ReportAddress == null || Report.ReportAddress.CompareTo(string.Empty) == 0)
                {
                    LocationText = "Location acquired but address not found.";
                    LocationTextColor = (Color)App.Current.Resources["SecondaryTextColor"];
                }
                else
                {
                    LocationText = Report.ReportAddress;
                    LocationTextColor = (Color)App.Current.Resources["PrimaryTextColor"];
                }
                GpsImage = "ic_gps_on.png";
            }
        }
        #endregion
    }
}
