using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using shared.Models;
using tremorur.Messages;
using tremorur.Models.Bluetooth;
using tremorur.Models.Bluetooth.Events;

public interface IBluetoothStateManager
{
    IBluetoothPeripheral? Peripheral { get; }
    bool IsConnected { get; }
    event EventHandler<CharacteristicValueChangedEventArgs>? CharacteristicValueChanged;
    event EventHandler<DiscoveredCharacteristicEventArgs>? DiscoveredCharacteristic;
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
        _bluetoothService.PeripheralDisconnected += OnPeripheralDisconnected;
        _bluetoothService.DiscoveredCharacteristic += OnDiscoveredCharacteristic;
        _bluetoothService.CharacteristicValueChanged += OnCharacteristicValueChanged;
        _bluetoothService.StartDiscovery(BluetoothIdentifiers.VibrationServiceUUID);
    }

    public event EventHandler<bool>? ConnectionStateChanged;
    public event EventHandler<IBluetoothPeripheral>? PeripheralConnected;
    public event EventHandler<CharacteristicValueChangedEventArgs>? CharacteristicValueChanged;
    public event EventHandler<DiscoveredCharacteristicEventArgs>? DiscoveredCharacteristic;
    public event EventHandler? PeripheralDisconnected;

    private bool IsValidPeripheral(IBluetoothPeripheral peripheral) => peripheral.Services.Any(s => s.UUID == BluetoothIdentifiers.VibrationServiceUUID);

    public void OnDiscoveredCharacteristic(object? sender, DiscoveredCharacteristicEventArgs e)
    {
        if (IsValidPeripheral(e.Peripheral))
        {
            _logger.LogInformation("Discovered characteristic: {UUID}", e.Characteristic.UUID);
            DiscoveredCharacteristic?.Invoke(this, e);
        }
    }
    public void OnCharacteristicValueChanged(object? sender, CharacteristicValueChangedEventArgs e)
    {
        if (IsValidPeripheral(e.Peripheral))
        {
            _logger.LogInformation("Characteristic value changed: {UUID}", e.Characteristic.UUID);
            CharacteristicValueChanged?.Invoke(this, e);
        }
    }
    private void OnPeripheralDisconnected(object? sender, PeripheralDisconnectedEventArgs e)
    {
        if (_peripheral != null && e.Peripheral == _peripheral)
        {
            _peripheral = null;
            _bluetoothService.StartDiscovery(BluetoothIdentifiers.VibrationServiceUUID);
            ConnectionStateChanged?.Invoke(this, false);
            PeripheralDisconnected?.Invoke(this, EventArgs.Empty);
            _messenger.SendMessage(new DeviceDisconnected());
        }
    }

    private async void OnDiscoveredPeripheral(object? sender, IDiscoveredPeripheral peripheral)
    {
        if (!peripheral.Services.Any(s => s == BluetoothIdentifiers.VibrationServiceUUID))
        {
            return;
        }

        _bluetoothService.StopDiscovery();
        _logger.LogInformation("Watch found: {Peripheral}", peripheral.LocalName);
        _peripheral = await peripheral.ConnectAsync();
        if (_peripheral == null)
        {
            _logger.LogError("Failed to connect to peripheral: {Peripheral}", peripheral.LocalName);
            return;
        }
        _logger.LogInformation("Connected to peripheral: {Peripheral}", _peripheral.Name);
        ConnectionStateChanged?.Invoke(this, true);
        PeripheralConnected?.Invoke(this, _peripheral);
        _messenger.SendMessage(new DeviceConnected(_peripheral));
    }
}