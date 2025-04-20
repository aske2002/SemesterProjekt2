using System.Diagnostics;
using System.Threading.Tasks;
using CoreBluetooth;
using Microsoft.Extensions.Logging;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;
public partial class BluetoothService
{
    private readonly ILogger<BluetoothService> _logger;
    private Dictionary<Guid, TaskCompletionSource<BluetoothPeripheral>?> connectTasks = new();
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
    }

    public partial bool IsScanning =>
        centralManager.IsScanning;

    private void CM_UpdatedState(object? sender, EventArgs e)
    {
        if (centralManager.State == CBManagerState.PoweredOn)
        {
            _logger.Log(LogLevel.Information, "Bluetooth is powered on.");
            _messenger.SendMessage(new BluetoothStateUpdated(BluetoothState.Available));
            if (shouldScan)
            {
                StartDiscovery();
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
        Guid identifier = new Guid(e.Peripheral.Identifier.ToString());
        var taskSource = connectTasks.GetValueOrDefault(identifier);
        if (taskSource != null)
        {
            var peripheral = new BluetoothPeripheral(e.Peripheral);
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
        var discoveredPeripheral = new DiscoveredPeripheral(e);
        AddDiscoveredPeripheral(discoveredPeripheral);
    }

    public partial async Task<BluetoothPeripheral> ConnectPeripheralAsync(DiscoveredPeripheral device)
    {

        var taskSource = new TaskCompletionSource<BluetoothPeripheral>();
        connectTasks.Add(device.UUID, taskSource);
        centralManager.ConnectPeripheral(device.NativePeripheral);
        return await taskSource.Task;
    }

    public partial void StartDiscovery()
    {
        if (IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan already in progress.");
            return;
        }
        shouldScan = true;
        centralManager.ScanForPeripherals(peripheralUuids: []);
    }

    public partial void StopDiscovery()
    {
        if (!IsScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan is not in progress.");
            return;
        }
        shouldScan = false;
        centralManager.StopScan();
    }

}