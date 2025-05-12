using CoreBluetooth;
using Foundation;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;
public partial class BluetoothService : IBluetoothService
{
    internal readonly ILogger<BluetoothService> _logger;
    private Dictionary<string, TaskCompletionSource<BluetoothPeripheral>?> connectTasks = new();
    CBCentralManager centralManager;
    public BluetoothService(IMessenger messenger, ILogger<BluetoothService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

        // Initialize the Bluetooth service
        centralManager = new CBCentralManager();
        centralManager.ConnectedPeripheral += CM_ConnectedPeripheral;
        centralManager.UpdatedState += CM_UpdatedState;
        centralManager.DiscoveredPeripheral += CM_DiscoveredPeripheral;
        centralManager.DisconnectedPeripheral += CM_DisconnectedPeripheral;
    }

    public event EventHandler<string> DisconnectedPeripheral = delegate { };

    public partial bool IsScanning =>
        centralManager.IsScanning;

    private void CM_DisconnectedPeripheral(object? sender, CBPeripheralErrorEventArgs e)
    {
        var identifier = e.Peripheral.Identifier.AsString();
        if (e.Error != null)
        {
            _logger.Log(LogLevel.Error, "Disconnected from peripheral: " + e.Peripheral.Name);
            DisconnectedPeripheral.Invoke(this, identifier);
        }
        else
        {
            _logger.Log(LogLevel.Information, "Disconnected from peripheral: " + e.Peripheral.Name);
            DisconnectedPeripheral.Invoke(this, identifier);
        }
    }

    private void CM_UpdatedState(object? sender, EventArgs e)
    {
        if (centralManager.State == CBManagerState.PoweredOn)
        {
            _logger.Log(LogLevel.Information, "Bluetooth is powered on.");
            _messenger.SendMessage(new BluetoothStateUpdated(BluetoothState.Available));
            if (shouldScan != null)
            {
                startInternalDiscovery();
            }
        }
        else
        {
            _logger.Log(LogLevel.Warning, "Bluetooth is not available.");
            _messenger.SendMessage(new BluetoothStateUpdated(BluetoothState.NotAvailable));
        }
    }
    private void CM_ConnectedPeripheral(object? sender, CBPeripheralEventArgs e)
    {
        string identifier = e.Peripheral.Identifier.ToString();
        var taskSource = connectTasks.GetValueOrDefault(identifier);
        if (taskSource != null)
        {
            var peripheral = new BluetoothPeripheral(e.Peripheral, this);
            taskSource.TrySetResult(peripheral);
            connectTasks.Remove(identifier);
        }
        else
        {
            _logger.Log(LogLevel.Information, "Connected to peripheral: " + e.Peripheral.Name);
        }
    }
    private void CM_DiscoveredPeripheral(object? sender, CBDiscoveredPeripheralEventArgs e)
    {
        var discoveredPeripheral = new DiscoveredPeripheral(e, this);
        AddDiscoveredPeripheral(discoveredPeripheral);
    }

    public partial async Task<IBluetoothPeripheral> ConnectPeripheralAsync(IDiscoveredPeripheral discoveredPeripheral)
    {
        if (discoveredPeripheral is not DiscoveredPeripheral bluetoothPeripheral)
        {
            throw new ArgumentException("Invalid peripheral type", nameof(discoveredPeripheral));
        }

        if (bluetoothPeripheral.NativePeripheral.State == CBPeripheralState.Connected)
        {
            _logger.Log(LogLevel.Information, "Peripheral is already connected.");
            return new BluetoothPeripheral(bluetoothPeripheral.NativePeripheral, this);
        }

        var taskSource = new TaskCompletionSource<BluetoothPeripheral>();
        connectTasks.Add(discoveredPeripheral.UUID, taskSource);
        centralManager.ConnectPeripheral(bluetoothPeripheral.NativePeripheral);
        return await taskSource.Task;
    }

    internal partial void startInternalDiscovery(string serviceUuid)
    {
        if (IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan already in progress.");
            stopInternalDiscovery();
        }

        if (ShouldScan != null && ShouldScan.Length > 0)
        {
            centralManager.ScanForPeripherals(shouldScan.Select(x => CBUUID.FromString(x)).ToArray(), options: null);
            _logger.Log(LogLevel.Information, "Bluetooth scan started for service UUIDs: " + string.Join(", ", shouldScan));
        } else 
        {
            centralManager.ScanForPeripherals(peripheralUuids: []);
            _logger.Log(LogLevel.Information, "Bluetooth scan started.");
        }
    }

    internal partial void stopInternalDiscovery()
    {
        if (!IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan is not in progress.");
            return;
        }
        shouldScan = null;
        centralManager.StopScan();
    }

}