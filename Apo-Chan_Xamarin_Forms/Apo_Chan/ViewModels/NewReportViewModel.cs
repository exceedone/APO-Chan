﻿using Apo_Chan.Geolocation;
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public class NewReportViewModel : BaseReportViewModel
    {
        #region Variable and Property
        public DelegateCommand SubmitCommand { get; private set; }
        #endregion

        #region Constructor
        public NewReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
            : base(navigationService, dialogService)
        {
            SubmitCommand = new DelegateCommand(submitReport);
        }
        #endregion

        #region Function
        
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

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
                    //if (!GlobalAttributes.isConnectedInternet)
                    //{
                    //    await dialogService.DisplayAlertAsync("Error", "APO-Chan cannot connect to the Internet!", "OK");
                    //    return;
                    //}
                    IsBusy = true;
                    Report.PropertyChanged -= OnDateTimeChanged;
                    try
                    {
                        await ReportManager.DefaultManager.SaveTaskAsync(Report);
                        GlobalAttributes.ShouldUpdateReports = true;

                        // has groupids, post 
                        if (!string.IsNullOrWhiteSpace(this.groupIds))
                        {
                            await CustomFunction.Post($"table/reportgroup/list/{this.Report.Id}", this.groupIds.Split(',').Select(x => new ReportGroupItem()
                            {
                                RefGroupId = x
                                    ,
                                RefReportId = Report.Id
                            }));
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("-------------------[Debug] NewReportViewModel > " + e.Message);
                    }
                    IsBusy = false;
                    //await this.navigateTop();
                    NavigationParameters parameters = new NavigationParameters();
                    parameters.Add("Reset", true);
                    await navigationService.GoBackAsync(parameters);
                }
            }
        }

        private int gpsStatus = 0;
        protected override void updateLocation()
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
