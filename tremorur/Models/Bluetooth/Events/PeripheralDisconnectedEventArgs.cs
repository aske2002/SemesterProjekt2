namespace tremorur.Models.Bluetooth.Events
{
    public partial class PeripheralDisconnectedEventArgs : EventArgs
    {
        public Exception? Error { get; }
        public IBluetoothPeripheral Peripheral { get; }
        public PeripheralDisconnectedEventArgs(IBluetoothPeripheral peripheral, Exception? error = null)
        {
            Peripheral = peripheral;
            Error = error;
        }
    }
}