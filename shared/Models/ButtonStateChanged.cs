
using System.Diagnostics.CodeAnalysis;
using shared.Models.Vibrations;

namespace shared.Models;

public record ButtonStateChanged(WatchButton Button, ButtonState State, DateTime Timestamp = default)
{
    public byte[] ToBytes()
    {
        return [Button.ToFlagByte(), State.ToFlagByte()];
    }

    public static bool TryParse(byte[] bytes, [NotNullWhen(true)] out ButtonStateChanged? result)
    {
        result = null;

        if (bytes.Length != 2)
            return false;

        try
        {
            result = FromBytes(bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static ButtonStateChanged FromBytes(byte[] bytes)
    {
        if (bytes.Length != 2)
            throw new ArgumentException("Invalid byte array length for ButtonStateChangedMessage");

        var button = VibrationHelpers.FromFlagByte<WatchButton>(bytes[0]);
        var state = VibrationHelpers.FromFlagByte<ButtonState>(bytes[1]);

        return new ButtonStateChanged(button, state);
    }
}