using Apo_Chan.Geolocation;
using Plugin.Geolocator.Abstractions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Position currentPosition { get; set; }
    }
}
