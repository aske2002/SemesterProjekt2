using client.Models;
using shared.Models.Vibrations;

public class SetVibrationSettingsMessage : AsyncRequestResponseMesage<VibrationSettings>
{
    public SetVibrationSettingsMessage(VibrationSettings value) : base(value)
    {
    }
}