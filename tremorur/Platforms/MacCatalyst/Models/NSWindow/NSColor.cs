using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

public class NSColor : NSObject
{

    public static NSColor Red => GetColor("redColor");
    public static NSColor Blue => GetColor("blueColor");
    public static NSColor Green => GetColor("greenColor");
    public static NSColor Yellow => GetColor("yellowColor");
    public static NSColor Black => GetColor("blackColor");
    public static NSColor White => GetColor("whiteColor");
    public static NSColor Clear => GetColor("clearColor");
    public static NSColor Gray => GetColor("grayColor");
    public static NSColor Orange => GetColor("orangeColor");
    public static NSColor Purple => GetColor("purpleColor");

    public override NativeHandle ClassHandle => CHelpers.objc_getClass("NSColor");
    public NSColor(IntPtr handle)
        : base(handle)
    {
    }

    private static NSColor GetColor(string selectorName)
    {
        var colorClass = Class.GetHandle("NSColor");
        var selector = Selector.GetHandle(selectorName);
        var colorPtr = CHelpers.Messaging.IntPtr_objc_msgSend(colorClass, selector);
        return Runtime.GetINativeObject<NSColor>(colorPtr, true);
    }

    public static NSColor FromHexWithAlpha(string hex, double alpha)
    {
        if (hex.Length != 7 || !hex.StartsWith("#"))
            throw new ArgumentException("Invalid hex color format. Use #RRGGBB.");

        var r = Convert.ToByte(hex.Substring(1, 2), 16) / 255.0;
        var g = Convert.ToByte(hex.Substring(3, 2), 16) / 255.0;
        var b = Convert.ToByte(hex.Substring(5, 2), 16) / 255.0;

        return FromRGBA((float)r, (float)g, (float)b, (float)alpha);
    }

    public static NSColor FromRGBA(float r, float g, float b, float a = 1.0f)
    {
        (nfloat nr, nfloat ng, nfloat nb, nfloat na) =
            (new nfloat(r), new nfloat(g), new nfloat(b), new nfloat(a));
        var nsColorClass = Class.GetHandle("NSColor");
        var selector = Selector.GetHandle("colorWithRed:green:blue:alpha:");
        var colorPtr = CHelpers.Messaging.IntPtr_objc_msgSend_nfloat_nfloat_nfloat_nfloat(
            nsColorClass, selector, nr, ng, nb, na);
        return Runtime.GetINativeObject<NSColor>(colorPtr, true);
    }

    public NSColor WithAlpha(float alpha)
    {
        IntPtr sel = CHelpers.sel_registerName("setAlphaComponent:");
        var value = new NFloat(alpha);
        IntPtr nsColorPtr = CHelpers.Messaging.objc_objc_msgSend_float(Handle, sel, value);
        return Runtime.GetNSObject<NSColor>(nsColorPtr) ?? throw new InvalidOperationException("Failed to create NSColor");
    }

    public float AlphaComponent
    {
        get
        {
            IntPtr sel = CHelpers.sel_registerName("alphaComponent");
            return CHelpers.Messaging.float_objc_msgSend(Handle, sel);
        }
    }

    public (float, float, float, float) RGBComponents
    {
        get
        {
            IntPtr sel = CHelpers.sel_registerName("getComponents:");
            var unsafeMutablePointerOfCgFloat = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NFloat)) * 4);
            CHelpers.Messaging.void_objc_msgSend_float_arr(Handle, sel, unsafeMutablePointerOfCgFloat);
            var components = new float[4];
            Marshal.Copy(unsafeMutablePointerOfCgFloat, components, 0, 4);
            Marshal.FreeHGlobal(unsafeMutablePointerOfCgFloat);
            return (components[0], components[1], components[2], components[3]);
        }
    }

    public float RedComponent
    {
        get
        {
            IntPtr sel = CHelpers.sel_registerName("redComponent");
            return CHelpers.Messaging.float_objc_msgSend(Handle, sel);
        }
    }

    public float GreenComponent
    {
        get
        {
            IntPtr sel = CHelpers.sel_registerName("greenComponent");
            return CHelpers.Messaging.float_objc_msgSend(Handle, sel);
        }
    }

    public float BlueComponent
    {
        get
        {
            IntPtr sel = CHelpers.sel_registerName("blueComponent");
            return CHelpers.Messaging.float_objc_msgSend(Handle, sel);
        }
    }
}