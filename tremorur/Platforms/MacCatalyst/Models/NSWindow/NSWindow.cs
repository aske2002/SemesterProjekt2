using Foundation;
using ObjCRuntime;
using CoreGraphics;
using UIKit;
using GameController;

public static class NSWindowCollectionBehavior
{
    public const nuint Default = 0;
    public const nuint CanJoinAllSpaces = 1 << 0; // 1
    public const nuint MoveToActiveSpace = 1 << 1; // 2
    public const nuint Managed = 1 << 2; // 4
    public const nuint Transient = 1 << 3; // 8
    public const nuint Stationary = 1 << 4; // 16
    public const nuint ParticipatesInCycle = 1 << 5; // 32
    public const nuint IgnoresCycle = 1 << 6; // 64
    public const nuint FullScreenPrimary = 1 << 7; // 128
    public const nuint FullScreenAuxiliary = 1 << 8; // 256
    public const nuint FullScreenNone = 1 << 9; // 512
    public const nuint FullScreenAllowsTiling = 1 << 11; // 2048
    public const nuint FullScreenDisallowsTiling = 1 << 12; // 4096
                                                            // ... and so on for other options
}

[Register("UINSWindow")]
public class UINSWindow : NSObject
{

    public UINSWindow(IntPtr handle) : base(handle) { }

    public NSWindowStyle StyleMask
    {
        get
        {
            var stylePtr = CHelpers.Messaging.IntPtr_objc_msgSend(Handle, Selector.GetHandle("styleMask"));
            return (NSWindowStyle)stylePtr;
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_UInt64(Handle, Selector.GetHandle("setStyleMask:"), (nuint)value);
        }
    }

    public string Title
    {
        get
        {
            var titlePtr = CHelpers.Messaging.IntPtr_objc_msgSend(Handle, Selector.GetHandle("title"));
            return NSString.FromHandle(titlePtr);
        }
        set
        {
            using var nsString = new NSString(value);
            CHelpers.Messaging.void_objc_msgSend_IntPtr(Handle, Selector.GetHandle("setTitle:"), nsString.Handle);
        }
    }

    public void PerformWindowDragWithEvent(UIEvent evt)
    {
        CHelpers.Messaging.void_objc_msgSend_UIEvent(Handle, Selector.GetHandle("performWindowDragWithEvent:"), evt.Handle);
    }

    public bool IsVisible
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("isVisible"));
        }
    }

    public bool IsKeyWindow
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("isKeyWindow"));
        }
    }

    public bool IsMainWindow
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("isMainWindow"));
        }
    }

    public bool Opaque
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("isOpaque"));
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_bool(Handle, Selector.GetHandle("setOpaque:"), value);
        }
    }

    public bool HasShadow
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("hasShadow"));
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_bool(Handle, Selector.GetHandle("setHasShadow:"), value);
        }
    }

    public nuint CollectionBehaviour
    {
        get
        {
            return CHelpers.Messaging.nuint_objc_msgSend(Handle, Selector.GetHandle("collectionBehavior"));
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_UInt64(Handle, Selector.GetHandle("setCollectionBehavior:"), value);
        }
    }

    public bool IsMoveableByWindowBackground
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("isMovableByWindowBackground"));
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_bool(Handle, Selector.GetHandle("setMovableByWindowBackground:"), value);
        }
    }

    public float AlphaValue
    {
        get
        {
            return CHelpers.Messaging.float_objc_msgSend(Handle, Selector.GetHandle("alphaValue"));
        }
        set
        {
            CHelpers.Messaging.void_objc_float(Handle, Selector.GetHandle("setAlphaValue:"), value);
        }
    }

    public bool TitlebarAppearsTransparent
    {
        get
        {
            return CHelpers.Messaging.bool_objc_msgSend(Handle, Selector.GetHandle("titlebarAppearsTransparent"));
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_bool(Handle, Selector.GetHandle("setTitlebarAppearsTransparent:"), value);
        }
    }

    public NSColor BackgroundColor
    {
        get
        {
            var nsColor = CHelpers.Messaging.IntPtr_objc_msgSend(Handle, Selector.GetHandle("backgroundColor"));
            return Runtime.GetINativeObject<NSColor>(nsColor, true);
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_IntPtr(Handle, Selector.GetHandle("setBackgroundColor:"), CHelpers.CreateNSColorFromCGColor(value.Handle));
        }
    }

    public CGRect Frame
    {
        get
        {
            return CHelpers.Messaging.CGRect_objc_msgSend_stret(Handle, Selector.GetHandle("frame"));
        }
        set
        {
            CHelpers.Messaging.void_objc_msgSend_CGRect(Handle, Selector.GetHandle("setFrame:"), value);
        }
    }

    public void MakeKeyAndOrderFront()
    {
        CHelpers.Messaging.void_objc_msgSend_IntPtr(Handle, Selector.GetHandle("makeKeyAndOrderFront:"), IntPtr.Zero);
    }

    public void OrderOut()
    {
        CHelpers.Messaging.void_objc_msgSend_IntPtr(Handle, Selector.GetHandle("orderOut:"), IntPtr.Zero);
    }

    public void Close()
    {
        CHelpers.Messaging.void_objc_msgSend(Handle, Selector.GetHandle("close"));
    }
}
