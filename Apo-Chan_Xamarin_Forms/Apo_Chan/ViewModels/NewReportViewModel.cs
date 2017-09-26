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
using Xamarin.Forms;

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

        private string locationText = string.Empty;
        public string LocationText
        {
            get
            {
                return locationText;
            }
            set
            {
                SetProperty(ref this.locationText, value);
            }
        }

        private Color locationTextColor;
        public Color LocationTextColor
        {
            get
            {
                return locationTextColor;
            }
            set
            {
                SetProperty(ref this.locationTextColor, value);
            }
        }

        private ImageSource gpsImage;
        public ImageSource GpsImage
        {
            get
            {
                return gpsImage;
            }
            set
            {
                SetProperty(ref this.gpsImage, value);
            }
        }

        public DelegateCommand SubmitCommand { get; private set; }
        public DelegateCommand UpdateLocationCommand { get; private set; }
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
            Report.PropertyChanged += OnDateTimeChanged;

            UpdateLocationCommand = new DelegateCommand(updateLocation);
            UpdateLocationCommand.Execute();
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
                    Report.PropertyChanged -= OnDateTimeChanged;
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

        private async void OnDateTimeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReportStartDate" || e.PropertyName == "ReportStartTime" ||
                e.PropertyName == "ReportEndDate" || e.PropertyName == "ReportEndTime")
            {
                DateTime start = new DateTime
                    (
                        Report.ReportStartDate.Year, Report.ReportStartDate.Month, Report.ReportStartDate.Day,
                        Report.ReportStartTime.Hours, Report.ReportStartTime.Minutes, Report.ReportStartTime.Seconds
                    );
                DateTime end = new DateTime
                    (
                        Report.ReportEndDate.Year, Report.ReportEndDate.Month, Report.ReportEndDate.Day,
                        Report.ReportEndTime.Hours, Report.ReportEndTime.Minutes, Report.ReportEndTime.Seconds
                    );
                if (!Extension.CheckDateTime(ref start, ref end))
                {
                    await dialogService.DisplayAlertAsync
                        (
                            "Error",
                            "The end time is earlier than the start time!",
                            "OK"
                        );
                }
                Report.ReportStartDate = start.Date;
                Report.ReportStartTime = start.TimeOfDay;
                Report.ReportEndDate = end.Date;
                Report.ReportEndTime = end.TimeOfDay;
            }
            else
            {
                return;
            }
        }

        private async void OnLocationAcquired(Position position)
        {
            if (Report != null)
            {
                Report.ReportLat = position.Latitude;
                Report.ReportLon = position.Longitude;
            }

            var result = await GeoService.DefaultInstance.GetAddressFromPositionAsync(position);
            if (result != null)
            {
                Report.ReportAddress = result;
                LocationText = Report.ReportAddress;
                LocationTextColor = (Color)App.Current.Resources["PrimaryTextColor"];
            }
            else
            {
                Report.ReportAddress = string.Empty;
                LocationText = "Location acquired but address not found.";
                LocationTextColor = (Color)App.Current.Resources["SecondaryTextColor"];
            }
            GpsImage = "ic_gps_new.png";

            GeoEvent.DefaultInstance.Unsubscribe(OnLocationAcquired);
        }

        private int gpsStatus = 0;
        private void updateLocation()
        {
            if (gpsStatus == 0)
            {
                GpsImage = "ic_gps_off.png";
                LocationText = "Please tap here to add location.";
                LocationTextColor = (Color)App.Current.Resources["SecondaryTextColor"];
            }
            else if (gpsStatus % 2 == 1)
            {
                GeoEvent.DefaultInstance.Subscribe(OnLocationAcquired);
                InitLocationServiceAsync();
            }
            else
            {
                if (GeoEvent.DefaultInstance.Contains(OnLocationAcquired))
                {
                    GeoEvent.DefaultInstance.Unsubscribe(OnLocationAcquired);
                }
                Report.ReportAddress = string.Empty;
                Report.ReportLat = 0;
                Report.ReportLon = 0;
                GpsImage = "ic_gps_off.png";
                LocationText = "No location information.";
                LocationTextColor = (Color)App.Current.Resources["SecondaryTextColor"];
            }
            gpsStatus++;
        }
        #endregion
    }
}
