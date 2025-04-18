using System.Collections.ObjectModel;
using tremorur.Messages;
using tremorur.Models.Bluetooth;

namespace tremorur.Services;

public partial class BluetoothService
{
    internal readonly IMessenger _messenger;
    public BluetoothService(IMessenger messenger)
    {
        _messenger = messenger;
    }

    private bool shouldScan = false;
    public ObservableCollection<DiscoveredPeripheral> DiscoveredPeripherals { get; } = new ObservableCollection<DiscoveredPeripheral>();
    internal void AddDiscoveredPeripheral(DiscoveredPeripheral discoveredPeripheral)
    {
        if (DiscoveredPeripherals.All(p => p.UUID != discoveredPeripheral.UUID))
        {
            DiscoveredPeripherals.Add(discoveredPeripheral);
            _messenger.SendMessage(new DiscoveredPeripheralMessage(discoveredPeripheral));
            DiscoveredPeripheral.Invoke(this, discoveredPeripheral);
        }

    }
    public event EventHandler<DiscoveredPeripheral> DiscoveredPeripheral = delegate { };
    public partial bool IsScanning { get; } // Defined per platform
    public partial Task<BluetoothPeripheral> ConnectPeripheralAsync(DiscoveredPeripheral device);
    public partial void StartDiscovery(); // Defined per platform
    public partial void StopDiscovery(); // Defined per platform
}