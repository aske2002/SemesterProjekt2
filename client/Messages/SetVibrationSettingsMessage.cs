using client.Models;
using shared.Models.Vibrations;

public class SetVibrationSettingsMessage : AsyncRequestResponseMesage<byte[]>
{
    public SetVibrationSettingsMessage(byte[] value) : base(value)
    {
    }
}