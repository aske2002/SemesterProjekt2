using System.Diagnostics;
using CoreBluetooth;
using Microsoft.Extensions.Logging;
using tremorur.Messages;

namespace tremorur.Services;
public partial class BluetoothService
{
    private readonly IMessenger _messenger;
    private readonly ILogger<BluetoothService> _logger;
    CBCentralManager centralManager;

    public BluetoothService(IMessenger messenger, ILogger<BluetoothService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));

        // Initialize the Bluetooth service
        centralManager = new CBCentralManager();
        centralManager.UpdatedState += (sender, e) =>
        {
            // Handle state updates
            _logger.Log(LogLevel.Information, "Bluetooth state updated: {State}", centralManager.State);
            if (centralManager.State == CBManagerState.PoweredOn)
            {
                _messenger.SendMessage(new BluetoothStateUpdated(BluetoothState.Available));
                if (_shouldScan)
                {
                    StartScan();
                }
            }
            else
            {
                _messenger.SendMessage(new BluetoothStateUpdated(BluetoothState.NotAvailable));
            }
        };
    }

    private partial bool _isScanning =>
        centralManager.IsScanning;

    partial void StartScan()
    {
        if (_isScanning)
        {
            _logger.Log(LogLevel.Information, "Bluetooth scan already in progress.");
            return;
        }

        centralManager.DiscoveredPeripheral += (sender, e) =>
        {
            var per = new BluetoothPeripheral(e.Peripheral);
            _logger.Log(LogLevel.Information, "Discovered peripheral: {Peripheral}", per);
        };
        centralManager.ScanForPeripherals(peripheralUuids: []);
    }

}