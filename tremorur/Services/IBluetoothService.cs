using System.Collections.ObjectModel;
using tremorur.Messages;
using tremorur.Models.Bluetooth;
using tremorur.Models.Bluetooth.Events;

namespace tremorur.Services;

public interface IBluetoothService
{
    ObservableCollection<IDiscoveredPeripheral> DiscoveredPeripherals { get; }
    event EventHandler<IDiscoveredPeripheral> DiscoveredPeripheral;
    bool IsScanning { get; }
    public Task<IBluetoothPeripheral> ConnectPeripheralAsync(IDiscoveredPeripheral device);
    public event EventHandler<CharacteristicValueChangedEventArgs> CharacteristicValueChanged;
    public event EventHandler<DiscoveredServiceEventArgs> DiscoveredService;
    public event EventHandler<DiscoveredCharacteristicEventArgs> DiscoveredCharacteristic;
    public event EventHandler<PeripheralDisconnectedEventArgs> PeripheralDisconnected;
    void StartDiscovery();
    void StartDiscovery(string[] serviceUUIDs);
    void StopDiscovery();
}