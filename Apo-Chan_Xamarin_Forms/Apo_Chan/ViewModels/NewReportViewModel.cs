using Apo_Chan.Items;
using Apo_Chan.Managers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Apo_Chan.ViewModels
{
    public class NewReportViewModel : BaseViewModel
    {
        #region Variable and Property
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

        public DelegateCommand SubmitCommand { get; private set; }
        #endregion

        #region Constructor
        public NewReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            Report = new ReportItem
            {
                Id = null,
                RefUserId = GlobalAttributes.refUserId,
                ReportStartDate = DateTime.Today,
                ReportStartTime = DateTime.Now.TimeOfDay,
                ReportEndDate = DateTime.Today,
                ReportEndTime = DateTime.Now.AddMinutes(30).TimeOfDay,
            };

            SubmitCommand = new DelegateCommand(submitReport);
            Report.PropertyChanged += checkDateTime;
        }
#endregion

#region Function
        private async void submitReport()
        {
            if (isValidReport())
            {
                var accepted = await dialogService.DisplayAlertAsync("Confirmation", "Do you want to submit this report?", "Confirm", "Cancel");
                if (accepted)
                {
                    IsBusy = true;
                    Report.PropertyChanged -= checkDateTime;
                    try
                    {
                        //using UTC when saving DateTime
                        Report.ReportEndDate = Report.ReportEndDate.Add(Report.ReportEndTime).ToUniversalTime();
                        Report.ReportStartDate = Report.ReportStartDate.Add(Report.ReportStartTime).ToUniversalTime();

                        await ReportManager.DefaultManager.SaveTaskAsync(Report);
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                    }
                    IsBusy = false;
                    await navigationService.GoBackAsync();
                }
            }
        }

        private bool isValidReport()
        {
            bool isValid = false;
            isValid = Report.ReportStartDate != null && Report.ReportStartTime != null;
            isValid &= Report.ReportEndDate != null && Report.ReportEndTime != null;
            isValid &= Report.ReportTitle != null;

            return isValid;
        }

        private async void checkDateTime(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReportStartDate" || e.PropertyName == "ReportStartTime" ||
                e.PropertyName == "ReportEndDate" || e.PropertyName == "ReportEndTime")
            {
                if (Report.ReportStartDate.CompareTo(Report.ReportEndDate) > 0)
                {
                    await dialogService.DisplayAlertAsync("Error", "The start date is later than the end date!", "OK");
                    Report.ReportEndDate = Report.ReportStartDate;
                }
                else if ((Report.ReportStartDate.CompareTo(Report.ReportEndDate) == 0)
                      && (Report.ReportStartTime.CompareTo(Report.ReportEndTime) > 0))
                {
                    await dialogService.DisplayAlertAsync("Error", "The start time is later than the end time!", "OK");
                    Report.ReportEndTime = Report.ReportStartTime.Add(TimeSpan.FromMinutes(30));
                }
            }
            else
            {
                return;
            }
        }
        #endregion
    }
}
