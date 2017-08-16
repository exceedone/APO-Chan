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
    public class DetailReportViewModel : BindableBase, INavigationAware
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
        public DetailReportViewModel(INavigationService navigationService, ReportItem report)
        {
            this.navigationService = navigationService;
            Report = report;

            UpdateCommand = new DelegateCommand(updateReport);
            DeleteCommand = new DelegateCommand(deleteReport);
        }

        private async void updateReport()
        {
            if (isValidReport())
            {
                ReportManager.DefaultManager.UpdateItem(Report);
                await navigationService.GoBackAsync();
            }
        }

        private async void deleteReport()
        {
            if (isValidReport())
            {
                ReportManager.DefaultManager.DeleteItem(Report);
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
            System.Diagnostics.Debug.WriteLine("------------------ OnNavigatedFrom DetailReport");
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            System.Diagnostics.Debug.WriteLine("------------------ OnNavigatedTo DetailReport");
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            System.Diagnostics.Debug.WriteLine("------------------ OnNavigatingTo DetailReport");
        }
    }
}
