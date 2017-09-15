using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLabs.Platform.Services.Geolocation;

namespace Apo_Chan
{
    public static class GlobalAttributes
    {
        public static string refUserId
        {
            get
            {
                return Apo_Chan.Items.UserItem.GetCachedUserItem().Id;
            }
        }

        private static IGeolocator geolocator;
        public static IGeolocator Geolocator
        {
            get
            {
                if (geolocator == null)
                {
                    geolocator = Xamarin.Forms.DependencyService.Get<IGeolocator>();
                    //geolocator.PositionError += OnListeningError;
                    //geolocator.PositionChanged += OnPositionChanged;
                }
                return geolocator;
            }
        }

        private static Position position;
        public static Position Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}
