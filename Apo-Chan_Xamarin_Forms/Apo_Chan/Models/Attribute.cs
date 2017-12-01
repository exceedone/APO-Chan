using Apo_Chan.Items;
using Plugin.Geolocator.Abstractions;
using Plugin.Connectivity;
using static Apo_Chan.Service.OfflineSync;

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

        public static Position currentPosition;
        public static Position CurrentPosition
        {
            get
            {
                if (currentPosition == null)
                {
                    currentPosition = new Position();
                }
                return currentPosition;
            }
        }

        public static bool IsConnectedInternet
        {
            get
            {
                return CrossConnectivity.Current.IsConnected;
            }
        }
    }
}
