[Flags]
public enum NSWindowStyle : ulong
{
    Borderless         = 0,
    Titled             = 1 << 0,
    Closable           = 1 << 1,
    Miniaturizable     = 1 << 2,
    Resizable          = 1 << 3,
    TexturedBackground = 1 << 8,
    UnifiedTitleAndToolbar = 1 << 12,
    FullScreen         = 1 << 14,
    FullSizeContentView = 1 << 15,
    UtilityWindow      = 1 << 4,
    DocModalWindow     = 1 << 6,
    NonactivatingPanel = 1 << 7,
    HUDWindow          = 1 << 13,
}