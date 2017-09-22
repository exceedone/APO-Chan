using Plugin.Geolocator.Abstractions;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apo_Chan.Models
{
    public sealed class GeoEvent : PubSubEvent<Position>
    {
        private static GeoEvent defaultInstance;

        static GeoEvent()
        {
            defaultInstance = new GeoEvent();
        }

        public static GeoEvent DefaultInstance
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
    }
}
