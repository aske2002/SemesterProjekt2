using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService
{
    private readonly CBService nativeService;

    public BluetoothPeripheralService(CBService cBService)
    {
        nativeService = cBService;
    }
    public partial List<BluetoothPeripheralCharacteristic> Characteristics
    {
        get
        {
            return this.nativeService.Characteristics.Select(c => new BluetoothPeripheralCharacteristic(c)).ToList();
        }
    }
    public partial string UUID => this.nativeService.UUID.ToString();
    public partial bool IsPrimary => this.nativeService.Primary;
}