using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using XLabs.Platform.Services.Geolocation;
using Android.Gms.Common.Apis;
using Android.Gms.Common;
using Android.Gms.Location;
using Apo_Chan.Droid.Geolocation;

using Android.Locations;
using Java.Lang;

[assembly: Xamarin.Forms.Dependency(typeof(FusedGeolocator))]

namespace Apo_Chan.Droid.Geolocation
{
    public class FusedGeolocator : Java.Lang.Object, IGeolocator, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private Position _lastPosition;

        private GeolocationContinuousListener _listener;

        private readonly LocationManager _manager;

        private readonly object _positionSync = new object();

        private readonly string[] _providers;

        private FusedListener fusedListener = null;

        public FusedGeolocator()
        {
            _manager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            _providers = _manager.GetProviders(false).Where(s => s != LocationManager.PassiveProvider).ToArray();

            mGoogleApiClient = new GoogleApiClient.Builder((Activity)Xamarin.Forms.Forms.Context)
                .AddApi(LocationServices.API)
                .AddConnectionCallbacks(this)
                .AddOnConnectionFailedListener(this)
                .Build();

            mGoogleApiClient.Connect();
        }

        public bool IsListening
        {
            get
            {
                return _listener != null;
            }
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
                return _providers.Length > 0;
            }
        }

        public bool IsGeolocationEnabled
        {
            get
            {
                return _providers.Any(_manager.IsProviderEnabled);
            }
        }

        public void StopListening()
        {
            if (_listener == null)
            {
                return;
            }

            _listener.PositionChanged -= OnListenerPositionChanged;
            _listener.PositionError -= OnListenerPositionError;

            for (var i = 0; i < _providers.Length; ++i)
            {
                _manager.RemoveUpdates(_listener);
            }

            _listener = null;
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

            if (!IsListening)
            {
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

                return fusedListener.Task;
            }

            // If we're already listening, just use the current listener
            lock (_positionSync)
            {
                if (_lastPosition == null)
                {
                    if (cancelToken != CancellationToken.None)
                    {
                        cancelToken.Register(() => tcs.TrySetCanceled());
                    }

                    EventHandler<PositionEventArgs> gotPosition = null;
                    gotPosition = (s, e) =>
                    {
                        tcs.TrySetResult(e.Position);
                        PositionChanged -= gotPosition;
                    };

                    PositionChanged += gotPosition;
                }
                else
                {
                    tcs.SetResult(_lastPosition);
                }
            }

            return tcs.Task;
        }

        public void StartListening(uint minTime, double minDistance)
        {
            StartListening(minTime, minDistance, false);
        }

        public void StartListening(uint minTime, double minDistance, bool includeHeading)
        {
            if (minTime < 0)
            {
                throw new ArgumentOutOfRangeException("minTime");
            }
            if (minDistance < 0)
            {
                throw new ArgumentOutOfRangeException("minDistance");
            }
            if (IsListening)
            {
                throw new InvalidOperationException("This Geolocator is already listening");
            }

            _listener = new GeolocationContinuousListener(_manager, TimeSpan.FromMilliseconds(minTime), _providers);
            _listener.PositionChanged += OnListenerPositionChanged;
            _listener.PositionError += OnListenerPositionError;

            var looper = Looper.MyLooper() ?? Looper.MainLooper;
            for (var i = 0; i < _providers.Length; ++i)
            {
                _manager.RequestLocationUpdates(_providers[i], minTime, (float)minDistance, _listener, looper);
            }
        }

        private void OnListenerPositionChanged(object sender, PositionEventArgs e)
        {
            if (!IsListening) // ignore anything that might come in afterwards
            {
                return;
            }

            lock (_positionSync)
            {
                _lastPosition = e.Position;

                var changed = PositionChanged;
                if (changed != null)
                {
                    changed(this, e);
                }
            }
        }

        private void OnListenerPositionError(object sender, PositionErrorEventArgs e)
        {
            StopListening();

            var error = PositionError;
            if (error != null)
            {
                error(this, e);
            }
        }

        internal static DateTimeOffset GetTimestamp(Location location)
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
            throw new NotImplementedException();
        }

        /*
         * GoogleApiClient.IOnConnectionFailedListener
         */
        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

    }
}