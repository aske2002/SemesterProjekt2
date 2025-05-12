using client.Services.Bluetooth.Core;
using Microsoft.Extensions.Logging;
using shared.Models;
using Tmds.DBus;

namespace client.Services.Bluetooth.Advertisements
{
    public class AdvertisingManager : IAsyncDisposable
    {
        private readonly ServerContext _Context;
        private Advertisement? _advertisement;
        private readonly ILogger<AdvertisingManager> _logger = CustomLoggingProvider.CreateLogger<AdvertisingManager>();

        public AdvertisingManager(ServerContext context)
        {
            _Context = context;
        }
        public async Task RegisterAdvertisement(Advertisement advertisement)
        {
            await _Context.Connection.RegisterObjectAsync(advertisement);
            _logger.LogInformation($"advertisement object {advertisement.ObjectPath} created");

            await GetAdvertisingManager().RegisterAdvertisementAsync(((IDBusObject)advertisement).ObjectPath,
                new Dictionary<string, object>());

            _logger.LogInformation($"advertisement {advertisement.ObjectPath} registered in BlueZ advertising manager");
        }

        private async Task UnregisterAdvertisement(Advertisement advertisement)
        {
            await GetAdvertisingManager().UnregisterAdvertisementAsync(advertisement.ObjectPath);
            _logger.LogInformation($"advertisement {advertisement.ObjectPath} unregistered in BlueZ advertising manager");
        }

        private ILEAdvertisingManager1 GetAdvertisingManager()
        {
            return _Context.Connection.CreateProxy<ILEAdvertisingManager1>("org.bluez", "/org/bluez/hci0");
        }

        public async Task CreateAdvertisement(AdvertisementProperties advertisementProperties, string applicationId)
        {
            _advertisement = new Advertisement($"/org/bluez/{applicationId}/advertisement0", advertisementProperties);
            await RegisterAdvertisement(_advertisement);
        }

        public async ValueTask DisposeAsync()
        {
            await UnregisterAdvertisement(_advertisement!);
            if (_advertisement != null)
            {
                await _advertisement.ReleaseAsync();
                _advertisement = null;
            }
        }
    }
}