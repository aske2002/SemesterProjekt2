using System.Diagnostics;
using CoreBluetooth;

namespace tremorur.Services;
public partial class BluetoothService
{
    CBCentralManager centralManager;

    public BluetoothService()
    {
        // Initialize the Bluetooth service
        centralManager = new CBCentralManager();
        centralManager.UpdatedState += (sender, e) =>
        {
            // Handle state updates
            Debug.WriteLine($"Bluetooth state updated: {centralManager.State}");

            if (centralManager.State == CBManagerState.PoweredOn)
            {
                if (_shouldScan)
                {
                    StartScan();
                }
            }
            else
            {
                Debug.WriteLine("Bluetooth is not available.");
            }
        };
    }

    private partial bool _isScanning
    {
        get => centralManager.IsScanning;
    }

    partial void StartScan()
    {
        if (_isScanning)
        {
            Debug.WriteLine("Already scanning.");
            return;
        }

        centralManager.DiscoveredPeripheral += (sender, e) =>
        {
            var per = new BluetoothPeripheral(e.Peripheral);
            Debug.WriteLine($"Discovered peripheral: {e.Peripheral.Name}");
        };
        centralManager.ScanForPeripherals(peripheralUuids: []);
    }

}