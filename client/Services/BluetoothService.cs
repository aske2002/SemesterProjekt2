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
        var conn = new Connection(Address.System);
        await conn.ConnectAsync();

        var service = new GattService();
        var charac = new MotorCharacteristic();

        await conn.RegisterObjectAsync(service);
        await conn.RegisterObjectAsync(charac);

        var gattManager = conn.CreateProxy<IGattManager1>("org.bluez", "/org/bluez/hci0");

        await gattManager.RegisterApplicationAsync(
            new ObjectPath("/org/bluez/example"),  // Application root, not the adapter path
            new Dictionary<string, object>());

        _logger.LogInformation("BLE GATT Service registered and running.");
        await Task.Delay(-1, stoppingToken); // Keep alive
    }
}
