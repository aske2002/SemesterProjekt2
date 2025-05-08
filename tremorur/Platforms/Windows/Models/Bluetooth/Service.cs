using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService : IBluetoothPeripheralService
{
    private readonly GattDeviceService nativeService;
    public BluetoothPeripheralService(GattDeviceService gattService)
    {
        nativeService = gattService;
        Initialize_Characteristics();
    }

    private async void Initialize_Characteristics()
    {
        var characteristicsResult = await nativeService.GetCharacteristicsAsync();
        var allCharacteristics = characteristicsResult.Characteristics.ToList();
        var missingCharacteristics = allCharacteristics.Where(x => !characteristics.Any(y => y.UUID == x.Uuid.ToString())).ToList();
        foreach (var characteristic in missingCharacteristics.Select(x => new BluetoothPeripheralCharacteristic(x)))
        {
            characteristics.Add(characteristic);
        }
    }

    private ObservableCollection<IBluetoothPeripheralCharacteristic> characteristics = new ObservableCollection<IBluetoothPeripheralCharacteristic>();
    public partial ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics => characteristics;
    public partial string UUID => nativeService.Uuid.ToString();
    public partial bool IsPrimary => false;
}