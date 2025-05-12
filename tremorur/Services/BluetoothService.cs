using System.Collections.ObjectModel;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;

public partial class BluetoothService : IBluetoothService
{
    internal readonly IMessenger _messenger;
    private bool shouldScan = false;
    public ObservableCollection<IDiscoveredPeripheral> DiscoveredPeripherals { get; } = new ObservableCollection<IDiscoveredPeripheral>();
    internal void AddDiscoveredPeripheral(DiscoveredPeripheral discoveredPeripheral)
    {
        if (DiscoveredPeripherals.All(p => p.UUID != discoveredPeripheral.UUID))
        {
            DiscoveredPeripherals.Add(discoveredPeripheral);
            _messenger.SendMessage(new DiscoveredPeripheralMessage(discoveredPeripheral));
            DiscoveredPeripheral.Invoke(this, discoveredPeripheral);
        }
    }
    internal void RemoveDiscoveredPeripheral(DiscoveredPeripheral discoveredPeripheral)
    {
        if (DiscoveredPeripherals.Contains(discoveredPeripheral))
        {
            DiscoveredPeripherals.Remove(discoveredPeripheral);
        }
    }
    

    public event EventHandler<IDiscoveredPeripheral> DiscoveredPeripheral = delegate { };
    public partial bool IsScanning { get; } // Defined per platform
    public partial Task<IBluetoothPeripheral> ConnectPeripheralAsync(IDiscoveredPeripheral discoveredPeripheral);
    public partial void StartDiscovery(); // Defined per platform
    public partial void StartDiscovery(string serviceUuid); // Defined per platform
    public partial void StopDiscovery(); // Defined per platform
}