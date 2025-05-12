using shared.Models;

namespace shared.Messages;

public record ButtonClickedMessage(WatchButton Button, DateTime Timestamp = default);
public record ButtonReleasedMessage(WatchButton Button, DateTime Timestamp = default);
public record ButtonPressedMessage(WatchButton Button, DateTime Timestamp = default);