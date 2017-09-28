using Apo_Chan.Geolocation;
using Plugin.Geolocator.Abstractions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Apo_Chan.Items;
namespace Apo_Chan
{
    public static class GlobalAttributes
    {
        public static UserItem User
        {
            get
            {
                return UserItem.GetCachedUserItem();
            }
        }
        public static string refUserId
        {
            get
            {
                return User.Id;
            }
        }

        public static Position currentPosition { get; set; }
    }
}
