namespace client.Services.Bluetooth.Gatt.Description
{
    [Flags]
    public enum CharacteristicFlags
    {
        Read = 1,
        Write = 2,
        WritableAuxiliaries = 4,
        Notify = 8,
        Indicate = 16,
        WriteWithoutResponse = 32,
    }
}