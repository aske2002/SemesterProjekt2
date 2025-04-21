using System.Diagnostics;

namespace tremorur.Models.Bluetooth;

public interface IBluetoothPeripheralService
{
    string UUID { get; }
    List<IBluetoothPeripheralCharacteristic> Characteristics { get; }
    bool IsPrimary { get; }
}