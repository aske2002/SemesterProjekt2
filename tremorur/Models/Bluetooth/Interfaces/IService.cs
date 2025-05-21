using System.Collections.ObjectModel;
using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheralService
{
    string UUID { get; }
    ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics { get; }
    event EventHandler<IBluetoothPeripheralCharacteristic> DiscoveredCharacteristic;
    // event EventHandler<IBluetoothPeripheralService> AvailabilityChanged;
    bool IsPrimary { get; }
}