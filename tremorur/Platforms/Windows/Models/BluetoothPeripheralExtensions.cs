namespace tremorur.Models;
public partial class BluetoothPeripheral
{

    public static BluetoothPeripheral FromCBPeripheral()
    {
        return new BluetoothPeripheral()
        {
            Name = cBPeripheral.Name
        };
    }
    private partial BluetoothPeripheralState getState()
    {
        return Bluetoot5hPeripheralState.Disconnected;
    }

}