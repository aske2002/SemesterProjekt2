﻿using System.Collections.Generic;
using System.Linq;
using client.Services.Bluetooth.Advertisements;

namespace client.Services.Bluetooth.Gatt.Description
{
    public class GattApplicationBuilder
    {
        private readonly IList<GattServiceBuilder> _serviceBuilders = new List<GattServiceBuilder>();
        public GattServiceBuilder AddService(GattServiceDescription gattServiceDescription)
        {
            var gattServiceBuilder = new GattServiceBuilder(gattServiceDescription);
            _serviceBuilders.Add(gattServiceBuilder);
            return gattServiceBuilder;
        }

        public IEnumerable<GattServiceDescription> BuildServiceDescriptions()
        {
            return _serviceBuilders.Select(s => s.ServiceDescription);
        }
    }
}