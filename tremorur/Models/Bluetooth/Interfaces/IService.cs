using System.Collections.ObjectModel;
using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheralService
{
    string UUID { get; }
    ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics { get; }
    bool IsPrimary { get; }
}