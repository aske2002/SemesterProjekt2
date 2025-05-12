using shared.Models;

namespace tremorur.Models;
public static class ButtonTypes
{
    public static List<BorderButton> CreateButtons(Services.IMessenger messenger) => Enum.GetValues(typeof(WatchButton))
            .Cast<WatchButton>()
            .Select(type => new BorderButton(messenger, type))
            .ToList();

    public static readonly Dictionary<WatchButton, string> Names = new()
    {
        { WatchButton.Up, "Up" },
        { WatchButton.Down, "Down" },
        { WatchButton.Ok, "Ok" },
        { WatchButton.Cancel, "Cancel" }
    };

    public static readonly Dictionary<WatchButton, double> Positions = new()
    {
        { WatchButton.Ok, 315 },
        { WatchButton.Cancel, 45 },
        { WatchButton.Down, 135 },
        { WatchButton.Up, 225 }
    };
}