namespace tremorur.Messages;

public enum WatchButton
{
    Up,
    Down,
    Ok,
    Cancel,
}

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
        { WatchButton.Ok, 45 },
        { WatchButton.Cancel, 135 },
        { WatchButton.Down, 225 },
        { WatchButton.Up, 315 }
    };
}

public record ButtonClickedEvent(WatchButton Button);
