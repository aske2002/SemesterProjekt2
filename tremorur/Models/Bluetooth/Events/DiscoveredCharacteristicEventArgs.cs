namespace tremorur.Models.Bluetooth.Events
{
    public partial class DiscoveredCharacteristicEventArgs : EventArgs
    {
        public IBluetoothPeripheral Peripheral { get; }
        public IBluetoothPeripheralService Service { get; }
        public IBluetoothPeripheralCharacteristic Characteristic { get; }
        public DiscoveredCharacteristicEventArgs(IBluetoothPeripheral peripheral, IBluetoothPeripheralService service, IBluetoothPeripheralCharacteristic characteristic)
        {
            Characteristic = characteristic;
            Peripheral = peripheral;
            Service = service;
        }
    }
}