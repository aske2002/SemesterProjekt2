namespace tremorur.Models.Bluetooth.Events
{
    public partial class CharacteristicValueChangedEventArgs : EventArgs
    {
        public IBluetoothPeripheral Peripheral { get; }
        public IBluetoothPeripheralService Service { get; }
        public IBluetoothPeripheralCharacteristic Characteristic { get; }
        public byte[] Value { get; }
        public CharacteristicValueChangedEventArgs(IBluetoothPeripheral peripheral, IBluetoothPeripheralService service, IBluetoothPeripheralCharacteristic characteristic, byte[] value)
        {
            Value = value;
            Peripheral = peripheral;
            Service = service;
            Characteristic = characteristic;
        }
    }
}