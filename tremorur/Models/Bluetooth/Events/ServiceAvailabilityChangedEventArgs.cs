namespace tremorur.Models.Bluetooth.Events
{
    public partial class ServiceAvailabilityChangedEventArgs : EventArgs
    {
        public IBluetoothPeripheralService Peripheral { get; }
        public ServiceAvailabilityChangedEventArgs(IBluetoothPeripheralService peripheral, Exception? error = null)
        {
            Peripheral = peripheral;
        }
    }
}