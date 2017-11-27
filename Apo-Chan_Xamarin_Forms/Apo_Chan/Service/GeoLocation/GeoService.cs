using Apo_Chan.Models;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;

namespace Apo_Chan.Geolocation
{
    public sealed class GeoService
    {
        private IGeolocator Geolocator
        {
            get
            {
                return CrossGeolocator.Current;
            }
        }

        private IFusedGeolocator FusedGeolocator
        {
            get
            {
                return DependencyService.Get<IFusedGeolocator>();
            }
        }

        private static GeoService defaultInstance;

        static GeoService()
        {
            defaultInstance = new GeoService();
        }

        public static GeoService DefaultInstance
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public static void Init()
        {
            //Init instance to connect Google API Client for FusedGeolocator
            if (Device.RuntimePlatform == Device.Android)
            {
                DefaultInstance.FusedGeolocator.DesiredAccuracy = 100;
            }
            else
            {
                DefaultInstance.Geolocator.DesiredAccuracy = 100;
            }
        }

        private async Task<bool> checkPermissionsAsync(Func<string, Task> alertOnViewModel)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                Debug.WriteLine("-------------------[Debug] GeoService > " +
                        "Currently does not have Location permissions, requesting permissions.");
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await alertOnViewModel("Location permission is required.");
                }

                var request = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                if (request.ContainsKey(Permission.Location))
                {
                    status = request[Permission.Location];
                }

                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("-------------------[Debug] GeoService > " +
                        "Location permission denied, can not get positions async.");

                    await alertOnViewModel("Location permission is denied.");
                    return false;
                }
            }

            return true;
        }

        public async Task GetPositionAsync(Func<string, Task> alertOnViewModel)
        {
            try
            {
                var hasPermission = await checkPermissionsAsync(alertOnViewModel);

                if (hasPermission)
                {
                    Position results = null;
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        //On Android, Fused Location Service has better performance
                        results = await FusedGeolocator.GetPositionAsync(10000);
                    }
                    else
                    {
                        results = await Geolocator.GetPositionAsync(TimeSpan.FromSeconds(10));
                    }
                    if (results != null)
                    {
                        GlobalAttributes.currentPosition = new Position
                        {
                            Latitude = results.Latitude,
                            Longitude = results.Longitude
                        };
                        GeoEvent.DefaultInstance.Publish(GlobalAttributes.currentPosition);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("-------------------[Debug] GeoService > " + ex.Message);
                await alertOnViewModel("Cannot acquire location information." +
                    "\nPlease enable location service and try again.");
            }
        }

        public async Task<string> GetAddressFromPositionAsync(Position position)
        {
            //Google Geocoding REST
            Position pos = new Position
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude,
            };
            return await Geocoding.GetAddressAsync(pos);
        }
    }
}
