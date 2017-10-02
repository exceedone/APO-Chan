using Apo_Chan.Items;
using Plugin.Geolocator.Abstractions;
using Plugin.Connectivity;

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

        public static bool isConnectedInternet
        {
            get
            {
                return CrossConnectivity.Current.IsConnected;
            }
        }
    }
}
