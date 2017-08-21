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
    public class NewReportViewModel : BindableBase
    {
        private INavigationService navigationService;
        public DelegateCommand SubmitCommand { get; private set; }

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
        public NewReportViewModel(INavigationService navigationService)
        {
            this.navigationService = navigationService;
            Report = new ReportItem
            {
                Id = null,
                RefUserId = GlobalAttributes.refUserId,
                ReportStartDate = DateTime.UtcNow.ToLocalTime(),
                ReportStartTime = DateTime.UtcNow.ToLocalTime().TimeOfDay,
                ReportEndDate = DateTime.UtcNow.ToLocalTime(),
                ReportEndTime = DateTime.UtcNow.ToLocalTime().AddMinutes(30).TimeOfDay,
            };

            SubmitCommand = new DelegateCommand(submitReport);
        }

        private async void submitReport()
        {
            if (isValidReport())
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

        private bool isValidReport()
        {
            bool isValid = false;
            isValid = Report.ReportStartDate != null && Report.ReportStartTime != null;
            isValid &= Report.ReportEndDate != null && Report.ReportEndTime != null;
            isValid &= Report.ReportTitle != null;
            isValid &= Report.ReportComment != null;

            return isValid;
        }
    }
}
