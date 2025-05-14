namespace tremorur.Models.Bluetooth.Events
{
    public partial class DiscoveredServiceEventArgs : EventArgs
    {
        public IBluetoothPeripheral Peripheral { get; }
        public IBluetoothPeripheralService Service { get; }
        public DiscoveredServiceEventArgs(IBluetoothPeripheral peripheral, IBluetoothPeripheralService service)
        {
            Peripheral = peripheral;
            Service = service;
        }
    }
}