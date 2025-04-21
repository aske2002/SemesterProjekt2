using System.Collections.ObjectModel;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;

public interface IBluetoothService
{
    ObservableCollection<IDiscoveredPeripheral> DiscoveredPeripherals { get; }
    event EventHandler<IDiscoveredPeripheral> DiscoveredPeripheral;
    bool IsScanning { get; }
    public Task<IBluetoothPeripheral> ConnectPeripheralAsync(IDiscoveredPeripheral device);
    void StartDiscovery();
    void StopDiscovery();
}