namespace tremorur.Models.Bluetooth.Events
{
    public partial class PeripheralConnectedEventArgs : EventArgs
    {
        public IBluetoothPeripheral Peripheral { get; }
        public PeripheralConnectedEventArgs(IBluetoothPeripheral peripheral)
        {
            Peripheral = peripheral;
        }
    }
}