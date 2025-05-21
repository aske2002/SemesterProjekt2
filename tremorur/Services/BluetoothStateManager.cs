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
        StartDiscovery();
    }

    public event EventHandler<bool>? ConnectionStateChanged;
    public event EventHandler<IBluetoothPeripheral>? PeripheralConnected;
    public event EventHandler<CharacteristicValueChangedEventArgs>? CharacteristicValueChanged;
    public event EventHandler<DiscoveredCharacteristicEventArgs>? DiscoveredCharacteristic;
    public event EventHandler? PeripheralDisconnected;

    private bool IsValidDiscoveredPeripheral(IDiscoveredPeripheral peripheral) => peripheral.Services.Any(s => s == BluetoothIdentifiers.TemorurServiceUUID);
    private bool IsValidPeripheral(IBluetoothPeripheral peripheral) => peripheral.Services.Any(s => s.UUID == BluetoothIdentifiers.TemorurServiceUUID);

    private void StartDiscovery()
    {
        _bluetoothService.StartDiscovery([BluetoothIdentifiers.TemorurServiceUUID]);
    }

    public async void OnDiscoveredCharacteristic(object? sender, DiscoveredCharacteristicEventArgs e)
    {
        _logger.LogWarning("Discovered characteristic: {Characteristic}", e.Characteristic.UUID);
        if (IsValidPeripheral(e.Peripheral))
        {
            if (e.Service.UUID == BluetoothIdentifiers.TemorurServiceUUID)
            {
                await e.Characteristic.SetNotifyingAsync(true);
            }

            DiscoveredCharacteristic?.Invoke(this, e);
        }
    }
    public void OnCharacteristicValueChanged(object? sender, CharacteristicValueChangedEventArgs e)
    {
        if (IsValidPeripheral(e.Peripheral))
        {
            CharacteristicValueChanged?.Invoke(this, e);
        }
    }
    private void OnPeripheralDisconnected(object? sender, PeripheralDisconnectedEventArgs e)
    {
        if (e.Peripheral.UUID == _peripheral?.UUID || _peripheral == null)
        {
            _peripheral = null;
            StartDiscovery();
            ConnectionStateChanged?.Invoke(this, false);
            PeripheralDisconnected?.Invoke(this, EventArgs.Empty);
            _messenger.SendMessage(new DeviceDisconnected());
        }
    }

    private async void OnDiscoveredPeripheral(object? sender, IDiscoveredPeripheral peripheral)
    {
        if (!IsValidDiscoveredPeripheral(peripheral))
        {
            _logger.LogInformation("Peripheral {Peripheral} does not have the required services", peripheral.LocalName);
            return;
        }

        _bluetoothService.StopDiscovery();
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