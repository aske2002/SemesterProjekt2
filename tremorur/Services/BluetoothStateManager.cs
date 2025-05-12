using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shared.Models;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

public interface IBluetoothStateManager
{
    IBluetoothPeripheral? Peripheral { get; }
}

public class BluetoothStateManager : IBluetoothStateManager
{
    private readonly ILogger<BluetoothStateManager> _logger;
    private readonly tremorur.Services.IMessenger _messenger;
    private readonly IBluetoothService _bluetoothService;
    private IBluetoothPeripheral? _peripheral;
    public IBluetoothPeripheral? Peripheral => _peripheral;

    public BluetoothStateManager(ILogger<BluetoothStateManager> logger, tremorur.Services.IMessenger messenger, IBluetoothService bluetoothService)
    {
        _logger = logger;
        _messenger = messenger;
        _bluetoothService = bluetoothService;
        _bluetoothService.DiscoveredPeripheral += OnDiscoveredPeripheral;
        _bluetoothService.StartDiscovery();
        _logger.LogInformation("Bluetooth discovery started");
    }

    public async void OnDiscoveredPeripheral(object? sender, IDiscoveredPeripheral peripheral)
    {
        foreach (var service in peripheral.Services)
        {
            _logger.LogInformation("Device {name} - Service {service}", peripheral.LocalName, service);
        }
        if (!peripheral.Services.Any(s => s == BluetoothIdentifiers.VibrationServiceUUID))
        {
            return;
        }
        _bluetoothService.StopDiscovery();
        _logger.LogInformation("Discovered peripheral: {Peripheral}", peripheral.Name);
        _peripheral = await peripheral.ConnectAsync();
        if (_peripheral == null)
        {
            _logger.LogError("Failed to connect to peripheral: {Peripheral}", peripheral.Name);
            return;
        }
        _logger.LogInformation("Connected to peripheral: {Peripheral}", _peripheral.Name);
        _messenger.SendMessage(new DeviceConnected(_peripheral));
        _peripheral.Disconnected += OnPeripheralDisconnected;
    }

    private void OnPeripheralDisconnected(object? sender, EventArgs e)
    {
        _logger.LogInformation("Peripheral disconnected: {Peripheral}", _peripheral?.Name);
        _peripheral = null;
        _bluetoothService.StartDiscovery();
        _messenger.SendMessage(new DeviceDisconnected());
    }
}