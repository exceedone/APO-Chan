using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Apo_Chan.ViewModels
{
    public class DetailReportViewModel : BindableBase, INavigatedAware
    {
        private INavigationService navigationService;
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }

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

        //constructor
        public DetailReportViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            UpdateCommand = new DelegateCommand(updateReport);
            DeleteCommand = new DelegateCommand(deleteReport);
        }

        private async void updateReport()
        {
            if (isValidReport())
            {
                try
                {
                    await ReportManager.DefaultManager.SaveTaskAsync(Report);
                }
                catch (Exception e)
                {

                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] ", e.Message);
                }
                await navigationService.GoBackAsync();
            }
        }

        private async void deleteReport()
        {
            Report.Deleted = true;
            try
            {
                await ReportManager.DefaultManager.SaveTaskAsync(Report);
            }
            catch (Exception e)
            {

                System.Diagnostics.Debug.WriteLine("-------------------[Debug] ", e.Message);
            }
            await navigationService.GoBackAsync();
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
                Report = await ReportManager.DefaultManager.GetItem((string)parameters["Id"]);
            }
        }
    }
}
