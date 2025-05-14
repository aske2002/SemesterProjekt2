using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using tremorur.Models.Bluetooth;
using tremorur.Models.Bluetooth.Events;

namespace tremorur.Services;

public partial class BluetoothService : IBluetoothService
{
    private ConcurrentDictionary<string, IBluetoothPeripheral> previouslyConnectedDevices = new ConcurrentDictionary<string, IBluetoothPeripheral>();
    internal readonly IMessenger _messenger;
    internal readonly ILogger<BluetoothService> _logger;
    internal string[]? ScanForUUIDs = null; //UUIDs to scan for, empty for all
    public ObservableCollection<IDiscoveredPeripheral> DiscoveredPeripherals { get; } = new ObservableCollection<IDiscoveredPeripheral>();
    internal void AddDiscoveredPeripheral(DiscoveredPeripheral discoveredPeripheral)
    {
        var existingPeripheralIndex = DiscoveredPeripherals.IndexOf(discoveredPeripheral);
        if (existingPeripheralIndex == -1)
        {
            DiscoveredPeripherals.Add(discoveredPeripheral);
        }
        else
        {
            DiscoveredPeripherals[existingPeripheralIndex] = discoveredPeripheral;
        }
        DiscoveredPeripheral.Invoke(this, discoveredPeripheral);
        _messenger.SendMessage(new DiscoveredPeripheralMessage(discoveredPeripheral));
    }
    internal void RemoveDiscoveredPeripheral(DiscoveredPeripheral discoveredPeripheral)
    {
        if (DiscoveredPeripherals.Contains(discoveredPeripheral))
        {
            DiscoveredPeripherals.Remove(discoveredPeripheral);
        }
    }

    internal void PeripheralDidDisconnect(string identifier, Exception? error)
    {
        if (previouslyConnectedDevices.TryGetValue(identifier, out var peripheral))
        {
            PeripheralDisconnected.Invoke(this, new PeripheralDisconnectedEventArgs(peripheral, error));
            _logger.LogInformation("Peripheral disconnected: {Peripheral}", peripheral.Name);
        }
    }

    public event EventHandler<CharacteristicValueChangedEventArgs> CharacteristicValueChanged = delegate { };
    public event EventHandler<DiscoveredServiceEventArgs> DiscoveredService = delegate { };
    public event EventHandler<DiscoveredCharacteristicEventArgs> DiscoveredCharacteristic = delegate { };
    public event EventHandler<PeripheralDisconnectedEventArgs> PeripheralDisconnected = delegate { };
    public event EventHandler<PeripheralConnectedEventArgs> PeripheralConnected = delegate { };
    public event EventHandler<IDiscoveredPeripheral> DiscoveredPeripheral = delegate { };
    public partial bool IsScanning { get; } // Defined per platform

    public async Task<IBluetoothPeripheral> ConnectPeripheralAsync(IDiscoveredPeripheral discoveredPeripheral)
    {
        if (previouslyConnectedDevices.TryGetValue(discoveredPeripheral.UUID, out var peripheral))
        {
            if (peripheral.IsConnected)
            {
                _logger.LogInformation("Already connected to peripheral: {Peripheral}", peripheral.Name);
                return peripheral;
            }

            await ConnectPeripheralAsyncInternal(peripheral);
            _logger.LogInformation("Reconnected to previously connected peripheral: {Peripheral}", peripheral.Name);

            return peripheral;
        }
        else
        {
            var newPeripheral = await ConnectPeripheralAsyncInternal(discoveredPeripheral);
            newPeripheral.DiscoveredService += IBluetoothPeripheral_DiscoveredService;

            if (newPeripheral != null)
            {
                previouslyConnectedDevices.TryAdd(newPeripheral.UUID, newPeripheral);
                _logger.LogInformation("Connected to new peripheral: {Peripheral}", newPeripheral.Name);

                return newPeripheral;
            }
            else
            {
                throw new Exception("Failed to connect to peripheral");
            }
        }
    }

    private void IBluetoothPeripheral_DiscoveredService(object? sender, IBluetoothPeripheralService service)
    {
        if (sender is IBluetoothPeripheral peripheral)
        {
            DiscoveredService?.Invoke(peripheral, new DiscoveredServiceEventArgs(peripheral, service));
            service.DiscoveredCharacteristic += (s, e) => IBluetoothPeripheralService_DiscoveredCharacteristic(s, peripheral, e);
        }
    }
    private void IBluetoothPeripheralService_DiscoveredCharacteristic(object? sender, IBluetoothPeripheral peripheral, IBluetoothPeripheralCharacteristic characteristic)
    {
        if (sender is IBluetoothPeripheralService service)
        {
            DiscoveredCharacteristic?.Invoke(peripheral, new DiscoveredCharacteristicEventArgs(peripheral, service, characteristic));
            characteristic.ValueUpdated += (s, e) => IBluetoothCharacteristic_ValueUpdated(s, peripheral, service, e);
        }
    }
    private void IBluetoothCharacteristic_ValueUpdated(object? sender, IBluetoothPeripheral peripheral, IBluetoothPeripheralService service, byte[] data)
    {
        if (sender is IBluetoothPeripheralCharacteristic characteristic)
        {
            CharacteristicValueChanged?.Invoke(characteristic, new CharacteristicValueChangedEventArgs(peripheral, service, characteristic, data));
        }
    }

    internal partial Task<IBluetoothPeripheral> ConnectPeripheralAsyncInternal(IDiscoveredPeripheral discoveredPeripheral);
    internal partial Task ConnectPeripheralAsyncInternal(IBluetoothPeripheral bluetoothPeripheral);


    public void StartDiscovery()
    {
        ScanForUUIDs = [];
        startInternalDiscovery();
    }

    public void StartDiscovery(string serviceUuid)
    {
        ScanForUUIDs = [serviceUuid];
        startInternalDiscovery();
    }

    public void StopDiscovery()
    {
        ScanForUUIDs = null;
        stopInternalDiscovery();
    }

    internal partial void startInternalDiscovery(); // Defined per platform
    internal partial void stopInternalDiscovery(); // Defined per platform
}