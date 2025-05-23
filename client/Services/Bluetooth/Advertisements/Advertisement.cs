﻿using System;
using System.Threading.Tasks;
using client.Services.Bluetooth.Core;
using Tmds.DBus;

namespace client.Services.Bluetooth.Advertisements
{
    public class Advertisement : PropertiesBase<AdvertisementProperties>, ILEAdvertisement1
    {
        public Advertisement(string objectPath, AdvertisementProperties properties) : base(objectPath, properties)
        {
        }

        public Task ReleaseAsync()
        {
            return Task.CompletedTask;
        }
    }
}