using System.Collections.ObjectModel;
using System.Diagnostics;
using CoreBluetooth;
using Foundation;

namespace tremorur.Models.Bluetooth;

public partial class BluetoothPeripheralService : IBluetoothPeripheralService
{
    private readonly CBService nativeService;
    private CBPeripheral? nativePeripheral => nativeService?.Peripheral;

    public BluetoothPeripheralService(CBService cBService)
    {
        nativeService = cBService;

        if (nativePeripheral != null)
        {
            nativePeripheral.DiscoveredCharacteristics += CBService_DiscoveredCharacteristics;
            nativePeripheral?.DiscoverCharacteristics(forService: cBService);
        }
    }

    private void CBService_DiscoveredCharacteristics(object? sender, CBServiceEventArgs e)
    {
        var allCharacteristics = nativeService?.Characteristics?.ToList() ?? new List<CBCharacteristic>();
        var missingCharacteristics = allCharacteristics.Where(x => !Characteristics.Any(y => y.UUID == x.UUID.ToString().ToLower())).ToList();

        foreach (var characteristic in missingCharacteristics.Select(x => new BluetoothPeripheralCharacteristic(x)))
        {
            DiscoveredCharacteristic?.Invoke(this, characteristic);
            characteristics.Add(characteristic);
        }
    }

    private ObservableCollection<IBluetoothPeripheralCharacteristic> characteristics = new ObservableCollection<IBluetoothPeripheralCharacteristic>();
    public partial ObservableCollection<IBluetoothPeripheralCharacteristic> Characteristics => characteristics;
    public partial string UUID => nativeService?.UUID.ToString().ToLower() ?? string.Empty;
    public partial bool IsPrimary => this.nativeService.Primary;
}