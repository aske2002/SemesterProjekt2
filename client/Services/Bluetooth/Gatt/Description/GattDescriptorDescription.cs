namespace client.Services.Bluetooth.Gatt.Description
{
    public class GattDescriptorDescription
    {
        public required byte[] Value { get; set; }
        public required string UUID { get; set; }
        public required string[] Flags { get; set; }
    }
}