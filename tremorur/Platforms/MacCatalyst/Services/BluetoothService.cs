using CoreBluetooth;
using Foundation;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;
public partial class BluetoothService : IBluetoothService
{
    private Dictionary<string, TaskCompletionSource<CBPeripheral>?> connectTasks = new();
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

    public event EventHandler<(string UUID, string? Error)> DisconnectedPeripheral = delegate { };

    public partial bool IsScanning =>
        centralManager.IsScanning;

    private void CM_DisconnectedPeripheral(object? sender, CBPeripheralErrorEventArgs e)
    {
        var identifier = e.Peripheral.Identifier.AsString();
        var code = e.Error?.Code.ToInt64() ?? 0;
        var code32 = e.Error?.Code.ToInt32();
        var error = e.Error != null ? new Exception(e.Error.LocalizedDescription) : null;
        PeripheralDidDisconnect(identifier, error);
    }

    private void CM_UpdatedState(object? sender, EventArgs e)
    {
        if (centralManager.State == CBManagerState.PoweredOn)
        {
            _logger.Log(LogLevel.Information, "Bluetooth is powered on.");
            _messenger.SendMessage(new BluetoothStateUpdated(BluetoothState.Available));
            if (ScanForUUIDs != null)
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
            taskSource.TrySetResult(e.Peripheral);
            connectTasks.Remove(identifier);
        }
    }
    private void CM_DiscoveredPeripheral(object? sender, CBDiscoveredPeripheralEventArgs e)
    {
        var discoveredPeripheral = new DiscoveredPeripheral(e, this);
        AddDiscoveredPeripheral(discoveredPeripheral);
    }

    internal partial async Task ConnectPeripheralAsyncInternal(IBluetoothPeripheral peripheral)
    {
        if (peripheral is not BluetoothPeripheral bluetoothPeripheral)
        {
            throw new ArgumentException("Invalid peripheral type", nameof(peripheral));
        }

        if (bluetoothPeripheral.NativePeripheral.State == CBPeripheralState.Connected)
        {
            _logger.Log(LogLevel.Information, "Peripheral is already connected.");
            return;
        }

        var taskSource = new TaskCompletionSource<CBPeripheral>();
        connectTasks.Add(peripheral.UUID, taskSource);
        centralManager.ConnectPeripheral(bluetoothPeripheral.NativePeripheral);
        await taskSource.Task;

    }

    internal partial async Task<IBluetoothPeripheral> ConnectPeripheralAsyncInternal(IDiscoveredPeripheral discoveredPeripheral)
    {
        if (discoveredPeripheral is not DiscoveredPeripheral bluetoothPeripheral)
        {
            throw new ArgumentException("Invalid peripheral type", nameof(discoveredPeripheral));
        }

        if (bluetoothPeripheral.NativePeripheral.State == CBPeripheralState.Connected)
        {
            _logger.Log(LogLevel.Information, "Peripheral is already connected.");
            return new BluetoothPeripheral(bluetoothPeripheral.NativePeripheral);
        }

        var taskSource = new TaskCompletionSource<CBPeripheral>();
        connectTasks.Add(discoveredPeripheral.UUID, taskSource);
        centralManager.ConnectPeripheral(bluetoothPeripheral.NativePeripheral);
        var nativePeripheral = await taskSource.Task;
        return new BluetoothPeripheral(nativePeripheral);
    }


    internal partial void startInternalDiscovery()
    {
        if (IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan already in progress.");
            stopInternalDiscovery();
        }

        if (ScanForUUIDs != null && ScanForUUIDs.Length > 0)
        {
            centralManager.ScanForPeripherals(ScanForUUIDs.Select(x => CBUUID.FromString(x)).ToArray());
            _logger.Log(LogLevel.Information, "Bluetooth scan started for service UUIDs: " + string.Join(", ", ScanForUUIDs));
        }
        else
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
        ScanForUUIDs = null;
        centralManager.StopScan();
    }

}