/*
 * Reference from: 
 * https://github.com/XLabs/Xamarin-Forms-Labs/wiki/Geolocator
 */


using System;

using Android.Locations;
using System.Threading.Tasks;
using System.Threading;
using Plugin.Geolocator.Abstractions;

namespace Apo_Chan.Droid.Geolocation
{
    internal class FusedListener : Java.Lang.Object, Android.Gms.Location.ILocationListener
    {
        private Location _bestLocation;

        private readonly TaskCompletionSource<Position> _completionSource = new TaskCompletionSource<Position>();

        private readonly float _desiredAccuracy;

        private readonly Action _finishedCallback;

        private readonly object _locationSync = new object();

        private readonly Timer _timer;

        public FusedListener(float desiredAccuracy, int timeout, Action finishedCallback)
        {
            _desiredAccuracy = desiredAccuracy;
            _finishedCallback = finishedCallback;

            if (timeout != Timeout.Infinite)
            {
                _timer = new Timer(TimesUp, null, timeout, 0);
            }
        }

        public Task<Position> Task
        {
            get
            {
                return _completionSource.Task;
            }
        }

        public void OnLocationChanged(Location location)
        {
            Models.DebugUtil.WriteLine("[Droid] OnLocationChanged > " + location.ToString());
            if (location.Accuracy <= _desiredAccuracy)
            {
                Finish(location);
                return;
            }

            lock (_locationSync)
            {
                if (_bestLocation == null || location.Accuracy <= _bestLocation.Accuracy)
                {
                    _bestLocation = location;
                }
            }
        }

        public void Cancel()
        {
            _completionSource.TrySetCanceled();
        }

        private void TimesUp(object state)
        {
            lock (_locationSync)
            {
                if (_bestLocation == null)
                {
                    if (_completionSource.TrySetCanceled() && _finishedCallback != null)
                    {
                        _finishedCallback();
                    }
                }
                else
                {
                    Finish(_bestLocation);
                }
            }
        }

        private void Finish(Location location)
        {
            var p = new Position();
            if (location.HasAccuracy)
            {
                p.Accuracy = location.Accuracy;
            }
            if (location.HasAltitude)
            {
                p.Altitude = location.Altitude;
            }
            if (location.HasBearing)
            {
                p.Heading = location.Bearing;
            }
            if (location.HasSpeed)
            {
                p.Speed = location.Speed;
            }

            p.Longitude = location.Longitude;
            p.Latitude = location.Latitude;
            p.Timestamp = FusedGeolocator.GetTimestamp(location);

            _finishedCallback?.Invoke();

            _completionSource.TrySetResult(p);
        }

    }
}