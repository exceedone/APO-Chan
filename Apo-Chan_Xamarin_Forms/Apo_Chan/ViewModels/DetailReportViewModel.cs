using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apo_Chan.ViewModels
{
    public class DetailReportViewModel : BaseViewModel, INavigatedAware
    {
        private ReportItem report;
        public ReportItem Report
        {
            get
            {
                return report;
            }
            set
            {
                SetProperty(ref this.report, value);
            }
        }

        public DelegateCommand UpdateCommand { get; private set; }

        public DelegateCommand DeleteCommand { get; private set; }

        //constructor
        public DetailReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            UpdateCommand = new DelegateCommand(updateReport);
            DeleteCommand = new DelegateCommand(deleteReport);
        }

        private async void updateReport()
        {
            if (isValidReport())
            {
                var accepted = await dialogService.DisplayAlertAsync("Confirmation", "Do you want to update the report?", "Confirm", "Cancel");
                if (accepted)
                {
                    try
                    {
                        await ReportManager.DefaultManager.SaveTaskAsync(Report);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                    }

                    await navigationService.GoBackAsync();
                }
            }
        }

        private async void deleteReport()
        {
            var accepted = await dialogService.DisplayAlertAsync("Confirmation", "Do you want to delete the report?", "Confirm", "Cancel");
            if (accepted)
            {
                try
                {
                    await ReportManager.DefaultManager.DeleteAsync(Report);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                }
                await navigationService.GoBackAsync();
            }
        }

        private bool isValidReport()
        {
            bool isValid = false;
            isValid = Report.ReportStartDate != null && Report.ReportStartTime != null;
            isValid &= Report.ReportEndDate != null && Report.ReportEndTime != null;
            isValid &= Report.ReportTitle != null;
            isValid &= Report.ReportComment != null;

            return isValid;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            ;
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Id"))
            {
                try
                {
                    Report = await ReportManager.DefaultManager.LookupAsync((string)parameters["Id"]);
                }
                catch (Exception e)
                {

                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                }
            }
        }
    }
}
