using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shared.Models;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

public interface IBluetoothStateManager
{
    IBluetoothPeripheral? Peripheral { get; }
    bool IsConnected { get; }
    event EventHandler<bool>? ConnectionStateChanged;
    event EventHandler<IBluetoothPeripheral>? PeripheralConnected;
    event EventHandler? PeripheralDisconnected;
}

public class BluetoothStateManager : IBluetoothStateManager
{
    private readonly ILogger<BluetoothStateManager> _logger;
    private readonly tremorur.Services.IMessenger _messenger;
    private readonly IBluetoothService _bluetoothService;
    private IBluetoothPeripheral? _peripheral;
    public IBluetoothPeripheral? Peripheral => _peripheral;
    public bool IsConnected => _peripheral != null;

    public BluetoothStateManager(ILogger<BluetoothStateManager> logger, tremorur.Services.IMessenger messenger, IBluetoothService bluetoothService)
    {
        _logger = logger;
        _messenger = messenger;
        _bluetoothService = bluetoothService;
        _bluetoothService.DiscoveredPeripheral += OnDiscoveredPeripheral;
        _bluetoothService.StartDiscovery();
    }
    public event EventHandler<bool>? ConnectionStateChanged;
    public event EventHandler<IBluetoothPeripheral>? PeripheralConnected;
    public event EventHandler? PeripheralDisconnected;
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
        ConnectionStateChanged?.Invoke(this, true);
        PeripheralConnected?.Invoke(this, _peripheral);
        _messenger.SendMessage(new DeviceConnected(_peripheral));
        _peripheral.Disconnected += OnPeripheralDisconnected;

    }

    private void OnPeripheralDisconnected(object? sender, EventArgs e)
    {
        _logger.LogInformation("Peripheral disconnected: {Peripheral}", _peripheral?.Name);
        _peripheral = null;
        _bluetoothService.StartDiscovery();
        ConnectionStateChanged?.Invoke(this, false);
        PeripheralDisconnected?.Invoke(this, EventArgs.Empty);
        _messenger.SendMessage(new DeviceDisconnected());
    }
}