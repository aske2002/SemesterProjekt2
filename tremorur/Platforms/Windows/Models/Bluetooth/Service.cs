using System.Diagnostics;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService
{
    private readonly GattDeviceService nativeService;
    public BluetoothPeripheralService(GattDeviceService  gattService)
    {
        nativeService = gattService;
        Initialize_Characteristics();
    }

    private async void Initialize_Characteristics()
    {
        var characteristicsResult = await nativeService.GetCharacteristicsAsync();
        var allCharacteristics = characteristicsResult.Characteristics.ToList();
        var missingCharacteristics = allCharacteristics.Where(x => !characteristics.Any(y => y.UUID == x.Uuid.ToString())).ToList();
        characteristics.AddRange(missingCharacteristics.Select(x => new BluetoothPeripheralCharacteristic(x)));
    }

    private List<BluetoothPeripheralCharacteristic> characteristics = new List<BluetoothPeripheralCharacteristic>();
    public partial List<BluetoothPeripheralCharacteristic> Characteristics => characteristics;
    public partial string UUID => nativeService.Uuid.ToString();
    public partial bool IsPrimary => false;
}