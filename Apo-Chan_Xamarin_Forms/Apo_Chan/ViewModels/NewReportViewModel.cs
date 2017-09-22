using Apo_Chan.Geolocation;
using Apo_Chan.Items;
using Apo_Chan.Managers;
using Apo_Chan.Models;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Events;
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

        private string reprotAddress = string.Empty;
        public string ReportAddress
        {
            get
            {
                return reprotAddress;
            }
            set
            {
                SetProperty(ref this.reprotAddress, value);
            }
        }

        public DelegateCommand SubmitCommand { get; private set; }
        #endregion

        #region Constructor
        public NewReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            // convert next 00, 10, 20, 30, 40, 50 minute
            var foonow = DateTime.Now;
            var now = new DateTime(foonow.Year, foonow.Month, foonow.Day, foonow.Hour, foonow.Minute, 0, 0, foonow.Kind);
            now = now.AddMinutes(10 - (now.Minute % 10));
            // get enddate
            var endDate = now.AddHours(1);
            Report = new ReportItem
            {
                Id = null,
                RefUserId = GlobalAttributes.refUserId,
                ReportStartDate = now.Date,
                ReportStartTime = now.TimeOfDay,
                ReportEndDate = endDate.Date,
                ReportEndTime = endDate.TimeOfDay,
            };

            SubmitCommand = new DelegateCommand(submitReport);
            Report.PropertyChanged += checkDateTime;

            GeoEvent.DefaultInstance.Subscribe(updateLocation);
            if (GeoService.DefaultInstance.IsAvailable)
            {
                InitLocationServiceAsync();
            }
        }
#endregion

#region Function
        private async void submitReport()
        {
            if (isValidReport())
            {
                var accepted = await dialogService.DisplayAlertAsync
                    (
                        "Confirmation",
                        "Do you want to submit this report?",
                        "Confirm",
                        "Cancel"
                    );
                if (accepted)
                {
                    IsBusy = true;
                    Report.PropertyChanged -= checkDateTime;
                    try
                    {
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
                    await dialogService.DisplayAlertAsync
                        (
                            "Error",
                            "The start date is later than the end date!",
                            "OK"
                        );
                    Report.ReportEndDate = Report.ReportStartDate;
                }
                else if ((Report.ReportStartDate.CompareTo(Report.ReportEndDate) == 0)
                      && (Report.ReportStartTime.CompareTo(Report.ReportEndTime) > 0))
                {
                    await dialogService.DisplayAlertAsync
                        (
                            "Error",
                            "The start time is later than the end time!",
                            "OK"
                        );
                    Report.ReportEndTime = Report.ReportStartTime.Add(TimeSpan.FromMinutes(30));
                }
            }
            else
            {
                return;
            }
        }

        private async void updateLocation(Position position)
        {
            if (Report != null)
            {
                Report.ReportLat = position.Latitude;
                Report.ReportLon = position.Longitude;
            }
            ReportAddress = await GeoService.DefaultInstance.GetAddressFromPositionAsync(position);
        }
        #endregion
    }
}
