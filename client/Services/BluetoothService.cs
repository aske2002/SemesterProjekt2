using Tmds.DBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using client.Services.Bluetooth;

namespace client.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string? systemBusAddress = Address.System;
        if (systemBusAddress is null)
        {
            Console.Write("Can not determine system bus address");
            return;
        }

        var conn = new Connection(Address.System);
        await conn.ConnectAsync();

        var objectManager = conn.CreateProxy<IObjectManager>("org.bluez", "/");
        bool hci0Exists = false;

        while (!hci0Exists)
        {
            var managedObjects = await objectManager.GetManagedObjectsAsync();
            hci0Exists = managedObjects.ContainsKey(new ObjectPath("/org/bluez/hci0"));

            if (!hci0Exists)
            {
                _logger.LogInformation("Waiting for Bluetooth adapter (hci0)...");
                await Task.Delay(1000);
            }
        }

        var service = new GattService();
        var charac = new MotorCharacteristic();

        await conn.RegisterObjectAsync(service);
        await conn.RegisterObjectAsync(charac);

        var gattManager = conn.CreateProxy<IGattManager1>("org.bluez", "/org/bluez/hci0");

        await gattManager.RegisterApplicationAsync(
            new ObjectPath("/org/bluez/tremorur"),  // Application root, not the adapter path
            new Dictionary<string, object>());

        _logger.LogInformation("BLE GATT Service registered and running.");
        await Task.Delay(-1, stoppingToken); // Keep alive
    }
}
