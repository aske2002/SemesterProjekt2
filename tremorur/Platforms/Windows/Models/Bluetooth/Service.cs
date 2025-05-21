using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService : IBluetoothPeripheralService
{
    private readonly GattDeviceService nativeService;
    public BluetoothPeripheralService(GattDeviceService gattService)
    {
        nativeService = gattService;
        nativeService.Session.SessionStatusChanged += (sender, args) =>
        {
            Console.WriteLine($"Session status changed: {args.Status}");
            if (args.Error != BluetoothError.Success)
            {
                Console.WriteLine($"Error: {args.Error}");
            }
        };
        Initialize_Characteristics();
    }
    private async void Initialize_Characteristics()
    {
        var access = await nativeService.RequestAccessAsync();
        nativeService.Session.MaintainConnection = true;
        nativeService.OpenAsync(GattSharingMode.SharedReadAndWrite);
        var characteristicsResult = await nativeService.GetCharacteristicsAsync();
        var allCharacteristics = characteristicsResult.Characteristics.ToList();
        var missingCharacteristics = allCharacteristics.Where(x => !characteristics.Any(y => y.UUID == x.Uuid.ToString())).ToList();
        foreach (var characteristic in missingCharacteristics.Select(x => new BluetoothPeripheralCharacteristic(x, nativeService)))
        {
            characteristics.Add(characteristic);
            DiscoveredCharacteristic.Invoke(this, characteristic);
        }
    }

    private ObservableCollection<IBluetoothPeripheralCharacteristic> characteristics = new ObservableCollection<IBluetoothPeripheralCharacteristic>();
    public partial ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics => characteristics;
    public partial string UUID => nativeService.Uuid.ToString().ToLower();
    public partial bool IsPrimary => false;
}