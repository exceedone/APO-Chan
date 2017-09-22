﻿using Apo_Chan.Items;
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

        public DelegateCommand UpdateCommand { get; private set; }

        public DelegateCommand DeleteCommand { get; private set; }

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
        #endregion

        #region Constructor
        public DetailReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            UpdateCommand = new DelegateCommand(updateReport);
            DeleteCommand = new DelegateCommand(deleteReport);

            GeoEvent.DefaultInstance.Subscribe(updateLocation);
            if (GeoService.DefaultInstance.IsAvailable)
            {
                InitLocationServiceAsync();
            }
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
                Report.PropertyChanged += checkDateTime;
            }
            else
            {
                await dialogService.DisplayAlertAsync("Error", "Failed to load the detail page!", "OK");
                await navigationService.GoBackAsync();
            }
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
