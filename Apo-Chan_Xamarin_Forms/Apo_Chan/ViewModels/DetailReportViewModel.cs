using Apo_Chan.Items;
using Apo_Chan.Managers;
using Apo_Chan.Geolocation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;
using Apo_Chan.Models;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class DetailReportViewModel : BaseViewModel, INavigatedAware
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

        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand UpdateLocationCommand { get; private set; }
        #endregion

        #region Constructor
        public DetailReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            UpdateCommand = new DelegateCommand(updateReport);
            DeleteCommand = new DelegateCommand(deleteReport);
            UpdateLocationCommand = new DelegateCommand(updateLocation);
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
                    if (!GlobalAttributes.isConnectedInternet)
                    {
                        await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                        return;
                    }
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
                if (!GlobalAttributes.isConnectedInternet)
                {
                    await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                    return;
                }
                IsBusy = true;
                try
                {
                    await ReportManager.DefaultManager.DeleteAsync(Report);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
                }
                await navigationService.GoBackAsync();
                IsBusy = false;
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

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            ;
        }

        public async void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Id"))
            {
                IsBusy = true;
                try
                {
                    Report = await ReportManager.DefaultManager.LookupAsync((string)parameters["Id"]);
                }
                catch (Exception e)
                {

                    System.Diagnostics.Debug.WriteLine("-------------------[Debug] " + e.Message);
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

        private async void OnDateTimeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReportStartDate" || e.PropertyName == "ReportStartTime" ||
                e.PropertyName == "ReportEndDate" || e.PropertyName == "ReportEndTime")
            {
                if (!Utils.CheckDateTimeContinuity(Report))
                {
                    await dialogService.DisplayAlertAsync
                        (
                            "Error",
                            "The end time is earlier than the start time!",
                            "OK"
                        );
                }
            }
            else
            {
                return;
            }
        }

        private void updateLocation()
        {
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
