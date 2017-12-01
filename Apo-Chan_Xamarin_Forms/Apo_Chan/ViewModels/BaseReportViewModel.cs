using Apo_Chan.Geolocation;
using Apo_Chan.Items;
using Apo_Chan.Models;
using Plugin.Geolocator.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Apo_Chan.ViewModels
{
    public abstract class BaseReportViewModel : BaseViewModel, INavigatedAware
    {
        #region Variable and Property
        protected ReportItem report;
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

        protected string locationText = string.Empty;
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

        protected Color locationTextColor;
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

        protected ImageSource gpsImage;
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

        protected string groupIds;
        protected string groupLabel;
        public string GroupLabel
        {
            get
            {
                if (string.IsNullOrWhiteSpace(groupLabel))
                {
                    return "No groups. Please tap selecting target groups.";
                }
                return groupLabel;
            }
            set
            {
                SetProperty(ref this.groupLabel, value);
            }
        }

        public DelegateCommand UpdateLocationCommand { get; protected set; }
        public DelegateCommand GroupSelectCommand { get; protected set; }

        #endregion

        #region Constructor
        public BaseReportViewModel(INavigationService navigationService, IPageDialogService dialogService)
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
                RefUserId = GlobalAttributes.User.Id,
                ReportStartDate = now.Date,
                ReportStartTime = now.TimeOfDay,
                ReportEndDate = endDate.Date,
                ReportEndTime = endDate.TimeOfDay,
            };

            Report.PropertyChanged += OnDateTimeChanged;

            UpdateLocationCommand = new DelegateCommand(updateLocation);
            UpdateLocationCommand.Execute();

            GroupSelectCommand = new DelegateCommand(selectGroup);
        }
        #endregion

        #region Function

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            ;
        }

        public virtual void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("GroupId") && parameters.ContainsKey("GroupName"))
            {
                this.groupIds = parameters["GroupId"].ToString();
                this.GroupLabel = Flurl.Url.DecodeQueryParamValue(parameters["GroupName"].ToString());
            }
        }

        protected bool isValidReport()
        {
            bool isValid = false;
            isValid = Report.ReportStartDate != null && Report.ReportStartTime != null;
            isValid &= Report.ReportEndDate != null && Report.ReportEndTime != null;
            isValid &= Report.ReportTitle != null;

            return isValid;
        }

        protected async void OnDateTimeChanged(object sender, PropertyChangedEventArgs e)
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

        protected async void OnLocationAcquired(Position position)
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

        protected abstract void updateLocation();

        /// <summary>
        /// select target group
        /// </summary>
        protected virtual async void selectGroup()
        {
            await navigationService.NavigateAsync($"GroupList?CalledType=2&GroupId={groupIds}");
        }
        #endregion
    }
}
