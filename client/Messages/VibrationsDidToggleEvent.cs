using CommunityToolkit.Mvvm.Messaging.Messages;

public class VibrationsDidToggleEvent : ValueChangedMessage<double>
{
    public VibrationsDidToggleEvent(double value) : base(value)
    {
    }
}