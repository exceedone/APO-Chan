/*
 * Reference from: 
 * https://github.com/XLabs/Xamarin-Forms-Labs/wiki/Geolocator
 */

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;

using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Apo_Chan.Droid.Geolocation;

using Android.Locations;
using Java.Lang;
using Plugin.Geolocator.Abstractions;
using Apo_Chan.Geolocation;

[assembly: Xamarin.Forms.Dependency(typeof(FusedGeolocator))]

namespace Apo_Chan.Droid.Geolocation
{
    public class FusedGeolocator : Java.Lang.Object, IFusedGeolocator, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //private readonly LocationManager _manager;

        //private readonly string[] _providers;

        private FusedListener fusedListener = null;

        public FusedGeolocator()
        {
            //_manager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            //_providers = _manager.GetProviders(false).Where(s => s != LocationManager.PassiveProvider).ToArray();

            mGoogleApiClient = new GoogleApiClient.Builder((Activity)Xamarin.Forms.Forms.Context)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            mGoogleApiClient.Connect();
        }

        public double DesiredAccuracy { get; set; }

        public bool SupportsHeading
        {
            get
            {
                return false;
            }
        }

        public bool IsGeolocationAvailable
        {
            get
            {
                if (mGoogleApiClient.IsConnected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                //return _providers.Length > 0;
            }
        }

        public bool IsGeolocationEnabled
        {
            get
            {
                return false;//do not use this
                //return _providers.Any(_manager.IsProviderEnabled);
            }
        }

        public event EventHandler<PositionErrorEventArgs> PositionError;

        public event EventHandler<PositionEventArgs> PositionChanged;

        public Task<Position> GetPositionAsync(CancellationToken cancelToken)
        {
            return GetPositionAsync(cancelToken, false);
        }

        public Task<Position> GetPositionAsync(CancellationToken cancelToken, bool includeHeading)
        {
            return GetPositionAsync(Timeout.Infinite, cancelToken);
        }

        public Task<Position> GetPositionAsync(int timeout)
        {
            return GetPositionAsync(timeout, false);
        }

        public Task<Position> GetPositionAsync(int timeout, bool includeHeading)
        {
            return GetPositionAsync(timeout, CancellationToken.None);
        }

        public Task<Position> GetPositionAsync(int timeout, CancellationToken cancelToken)
        {
            return GetPositionAsync(timeout, cancelToken, false);
        }

        public Task<Position> GetPositionAsync(int timeout, CancellationToken cancelToken, bool includeHeading)
        {
            if (timeout <= 0 && timeout != Timeout.Infinite)
            {
                throw new ArgumentOutOfRangeException("timeout", "timeout must be greater than or equal to 0");
            }

            var tcs = new TaskCompletionSource<Position>();

            fusedListener = new FusedListener(
                (float)DesiredAccuracy,
                timeout,
                () =>
                {
                    try
                    {
                        LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, fusedListener);
                        System.Diagnostics.Debug.WriteLine("-------------------[Debug.Droid] " + "FusedListener > FinishCallback");
                    }
                    catch (System.Exception ex)
                    {
                        tcs.SetException(ex);
                        System.Diagnostics.Debug.WriteLine("-------------------[Debug.Droid] " + ex.Message);
                    }
                });

            if (cancelToken != CancellationToken.None)
            {
                cancelToken.Register(
                    () =>
                    {
                        fusedListener.Cancel();
                        if (mGoogleApiClient.IsConnected)
                        {
                            LocationServices.FusedLocationApi.RemoveLocationUpdates(mGoogleApiClient, fusedListener);
                        }
                    },
                    true);
            }

            if (mGoogleApiClient.IsConnected)
            {
                try
                {
                    LocationServices.FusedLocationApi.RequestLocationUpdates(mGoogleApiClient, mLocationRequest, fusedListener);
                }
                catch (SecurityException ex)
                {
                    tcs.SetException(new GeolocationException(GeolocationError.Unauthorized, ex));
                    return tcs.Task;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("-------------------[Debug.Droid] " + "Google Api Client not connected");
            }

            return fusedListener.Task;
        }

        internal static DateTimeOffset GetTimestamp(Android.Locations.Location location)
        {
            return new DateTimeOffset(Epoch.AddMilliseconds(location.Time));
        }

        /*
         * GoogleApiClient.IConnectionCallbacks
         */
        private GoogleApiClient mGoogleApiClient;

        private LocationRequest mLocationRequest;

        public void OnConnected(Bundle connectionHint)
        {
            mLocationRequest = LocationRequest.Create();
            mLocationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            mLocationRequest.SetInterval(1000);

            System.Diagnostics.Debug.WriteLine("-------------------[Debug.Droid] " + "GoogleApiClient > OnConnected");
        }

        public void OnConnectionSuspended(int cause)
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug.Droid] " + "GoogleApiClient > Connection Suspended");
        }

        /*
         * GoogleApiClient.IOnConnectionFailedListener
         */
        public void OnConnectionFailed(ConnectionResult result)
        {
            System.Diagnostics.Debug.WriteLine("-------------------[Debug.Droid] " + "GoogleApiClient > Connection Failed");
        }

    }
}