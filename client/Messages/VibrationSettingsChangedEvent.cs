using client.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;
using shared.Models.Vibrations;

public class VibrationSettingsChangedEvent : ValueChangedMessage<VibrationSettings>
{
    public VibrationSettingsChangedEvent(VibrationSettings value) : base(value)
    {
    }
}