using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CoreGraphics;
using Foundation;
using GameController;
using ObjCRuntime;
using UIKit;


public static class CHelpers
{
    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "class_respondsToSelector")]
    public static extern bool class_respondsToSelector(IntPtr cls, IntPtr selector);
    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_getClass")]
    public static extern IntPtr objc_getClass(string className);

    [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
    public static extern IntPtr sel_registerName(string selectorName);


    [DllImport("/usr/lib/libobjc.dylib")]
    public static extern IntPtr object_getClass(IntPtr obj);

    [DllImport("/usr/lib/libobjc.dylib")]
    public static extern IntPtr class_getName(IntPtr cls);

    [DllImport("/usr/lib/libobjc.dylib")]
    public static extern IntPtr method_getName(IntPtr method);

    [DllImport("/usr/lib/libobjc.dylib")]
    public static extern IntPtr sel_getName(IntPtr selector);

    public static string? GetClassName(IntPtr cls)
    {
        var namePtr = class_getName(cls);
        return Marshal.PtrToStringAnsi(namePtr);
    }

    public static IntPtr CreateCGColorFromNSColor(IntPtr nsColor)
    {
        IntPtr selGCColor = sel_registerName("CGColor");
        return Messaging.IntPtr_objc_msgSend(nsColor, selGCColor);
    }

    public static IntPtr CreateNSColorFromCGColor(IntPtr cgColor)
    {
        IntPtr nsColorClass = objc_getClass("NSColor");
        IntPtr selColorWithCGColor = sel_registerName("colorWithCGColor:");
        return Messaging.objc_msgSend_IntPtr(nsColorClass, selColorWithCGColor, cgColor);
    }

    public static string? GetSuperClassName(IntPtr cls)
    {
        var superClass = PropertyHelpers.class_getSuperclass(cls);
        var namePtr = class_getName(superClass);
        return Marshal.PtrToStringAnsi(namePtr);
    }

    public static IntPtr GetNSApp()
    {
        IntPtr nsAppClass = objc_getClass("NSApplication");
        IntPtr sharedAppSelector = sel_registerName("sharedApplication");
        return Messaging.IntPtr_objc_msgSend(nsAppClass, sharedAppSelector);
    }

    public static IntPtr GetNSAppDelegate()
    {
        var app = GetNSApp();
        return Messaging.IntPtr_objc_msgSend(app, sel_registerName("delegate"));
    }

    public static class Messaging
    {
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr IntPtr_objc_msgSend_nfloat_nfloat_nfloat_nfloat(
            IntPtr receiver,
            IntPtr selector,
            nfloat red,
            nfloat green,
            nfloat blue,
            nfloat alpha
        );

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_UIEvent(
            IntPtr receiver,
            IntPtr selector,
            IntPtr eventType
        );

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern bool void_objc_float(IntPtr receiver, IntPtr selector, float value);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern bool bool_objc_msgSend(IntPtr receiver, IntPtr selector);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_objc_msgSend_float(IntPtr receiver, IntPtr selector, NFloat value);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_float_arr(IntPtr receiver, IntPtr selector, IntPtr array);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern float float_objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool value);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern CGRect CGRect_objc_msgSend_stret(IntPtr receiver, IntPtr selector);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_CGRect(IntPtr receiver, IntPtr selector, CGRect frame);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]

        public static extern nuint void_objc_msgSend_UInt64(IntPtr receiver, IntPtr selector, nuint arg1);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern nuint nuint_objc_msgSend(IntPtr receiver, IntPtr selector);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]

        public static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern void objc_msgSend_bool(IntPtr receiver, IntPtr selector, bool arg1);

        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);
        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

        [DllImport("/usr/lib/libobjc.A.dylib", EntryPoint = "objc_msgSend")]
        public static extern void void_objc_msgSend(IntPtr receiver, IntPtr selector);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        public static extern IntPtr objc_msgSend_color(
    IntPtr receiver,
    IntPtr selector,
    double red,
    double green,
    double blue,
    double alpha
);
    }

    public static class PropertyHelpers
    {
        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr class_copyPropertyList(IntPtr cls, out uint count);
        [DllImport("/usr/lib/libobjc.dylib", EntryPoint = "class_getSuperclass")]
        public static extern IntPtr class_getSuperclass(IntPtr cls);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr property_getName(IntPtr property);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr property_getAttributes(IntPtr property);
    }

    public static class MethodHelpers
    {
        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr class_copyMethodList(IntPtr cls, out uint count);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr method_getName(IntPtr method);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr method_getTypeEncoding(IntPtr method);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr method_copyReturnType(IntPtr method);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern IntPtr method_copyArgumentType(IntPtr method, uint index);

        [DllImport("/usr/lib/libobjc.dylib")]
        public static extern uint method_getNumberOfArguments(IntPtr method);

    }


    public static void PrintAllMethods(IntPtr obj)
    {
        IntPtr cls = object_getClass(obj);
        uint count;
        IntPtr methodListPtr = MethodHelpers.class_copyMethodList(cls, out count);

        for (uint i = 0; i < count; i++)
        {
            IntPtr methodPtr = Marshal.ReadIntPtr(methodListPtr, (int)(i * IntPtr.Size));
            IntPtr selector = method_getName(methodPtr);
            string? selName = Marshal.PtrToStringAnsi(sel_getName(selector));
            Debug.WriteLine($"  - {selName}");
        }
    }
}



public static class NSObjectHelper
{

    public record MethodInfo(string Name, string Encoding, string ReturnType, uint ArgumentCount, string[] ArgumentTypes)
    {
        public void Write(TextWriter writer, int indentLevel = 0)
        {
            // Indent the output based on the level
            string indent = new string(' ', indentLevel * 4);
            writer.WriteLine($"{indent}{Name}({string.Join(", ", ArgumentTypes)}): {ReturnType}");
        }
    }
    public record PropertyInfo(string Name, string Attributes, string SuperClassName, string Type, List<PropertyInfo>? SubProperties = null, List<MethodInfo>? methods = null)
    {
        public PropertyInfo WriteToFile(string filePath)
        {
            using (TextWriter writer = new StreamWriter(filePath, true))
            {
                Write(writer);
            }
            return this;
        }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                Write(writer);
                return writer.ToString();
            }
        }

        public void Write(TextWriter? writer, int indentLevel = 0)
        {
            // Indent the output based on the level
            string indent = new string(' ', indentLevel * 4);
            string semiIndentLevel = new string(' ', indentLevel * 2);
            writer ??= new StringWriter();

            writer.WriteLine($"{indent}Property: {Name}");
            writer.WriteLine($"{indent}{semiIndentLevel}Attributes: {Attributes}");
            writer.WriteLine($"{indent}{semiIndentLevel}Type: {Type}");
            writer.WriteLine($"{indent}{semiIndentLevel}SuperClass: {SuperClassName}");
            if (methods != null)
            {
                writer.WriteLine($"{indent}{semiIndentLevel}Methods:");
                foreach (var method in methods)
                {
                    method.Write(writer, indentLevel + 1);
                }
            }

            if (SubProperties != null)
            {
                writer.WriteLine($"{indent}{semiIndentLevel}SubProperties:");
                foreach (var subProperty in SubProperties)
                {
                    subProperty.Write(writer, indentLevel + 1);
                }
            }
        }
    }

    public static T? Property<T>(this NSObject obj, string propertyName) where T : NSObject
    {
        IntPtr propertySelector = CHelpers.sel_registerName(propertyName);
        IntPtr propertyValue = CHelpers.Messaging.IntPtr_objc_msgSend(obj.Handle, propertySelector);
        return Runtime.GetNSObject<T>(propertyValue);
    }

    public static PropertyInfo GetClassInfo(this NSObject obj, int maxDepth = 1)
    {

        IntPtr cls = GetActualClass(CHelpers.object_getClass(obj.Handle));
        string className = CHelpers.GetClassName(cls) ?? "(null)";
        string superClassName = CHelpers.GetSuperClassName(cls) ?? "(null)";

        var (properties, methods) = GetAllPropertiesRecursive(cls, maxDepth);

        return new PropertyInfo(className, "", superClassName, "ROOT", properties, methods);
    }


    public static List<MethodInfo> GetAllMethods(IntPtr classPtr)
    {
        IntPtr cls = CHelpers.object_getClass(classPtr);
        uint methodCount;
        IntPtr methodListPtr = CHelpers.MethodHelpers.class_copyMethodList(cls, out methodCount);
        List<MethodInfo> methods = new List<MethodInfo>();

        for (int i = 0; i < methodCount; i++)
        {
            IntPtr methodPtr = Marshal.ReadIntPtr(methodListPtr, i * IntPtr.Size);

            IntPtr sel = CHelpers.MethodHelpers.method_getName(methodPtr);
            string methodName = Marshal.PtrToStringAnsi(CHelpers.sel_getName(sel));

            IntPtr encodingPtr = CHelpers.MethodHelpers.method_getTypeEncoding(methodPtr);
            string encoding = Marshal.PtrToStringAnsi(encodingPtr);

            IntPtr returnTypePtr = CHelpers.MethodHelpers.method_copyReturnType(methodPtr);
            string returnType = Marshal.PtrToStringAnsi(returnTypePtr);

            uint argCount = CHelpers.MethodHelpers.method_getNumberOfArguments(methodPtr);

            string[] argTypes = new string[argCount];

            for (uint arg = 0; arg < argCount; arg++)
            {
                IntPtr argTypePtr = CHelpers.MethodHelpers.method_copyArgumentType(methodPtr, arg);
                string argType = Marshal.PtrToStringAnsi(argTypePtr);
                argTypes[arg] = DecodeArgumentType(argType);
            }

            methods.Add(new MethodInfo(methodName, encoding, DecodeArgumentType(returnType), argCount, argTypes));
        }

        // Free unmanaged memory
        Marshal.FreeHGlobal(methodListPtr);
        return methods;
    }

    public static (List<PropertyInfo> properties, List<MethodInfo> methods) GetAllPropertiesRecursive(IntPtr classPtr, int maxDepth = 1, int currentDepth = 1)
    {
        var allProps = new List<PropertyInfo>();
        var allMethods = new List<MethodInfo>();

        while (classPtr != IntPtr.Zero)
        {
            allProps.AddRange(GetAllPropertiesForClass(classPtr, maxDepth, currentDepth));
            allMethods.AddRange(GetAllMethods(classPtr));
            classPtr = CHelpers.PropertyHelpers.class_getSuperclass(classPtr); // Use class_getSuperclass, not objc_getSuperclass
        }

        return (allProps, allMethods);
    }

    public static List<PropertyInfo> GetAllPropertiesForClass(IntPtr classPtr, int maxDepth = 1, int currentDepth = 1)
    {
        uint count;
        IntPtr propListPtr = CHelpers.PropertyHelpers.class_copyPropertyList(classPtr, out count);
        List<PropertyInfo> properties = new List<PropertyInfo>();

        for (int i = 0; i < count; i++)
        {
            IntPtr propertyPtr = Marshal.ReadIntPtr(propListPtr, i * IntPtr.Size);

            string name = Marshal.PtrToStringAnsi(CHelpers.PropertyHelpers.property_getName(propertyPtr)) ?? "(null)";
            string attributes = Marshal.PtrToStringAnsi(CHelpers.PropertyHelpers.property_getAttributes(propertyPtr)) ?? "";

            string typeInfo = DecodePropertyType(attributes);

            if (currentDepth <= maxDepth && attributes.StartsWith("T@\""))
            {
                IntPtr cls = CHelpers.objc_getClass(attributes[3..attributes.IndexOf('\"', 3)]);
                IntPtr subClassPtr = GetActualClass(cls);

                if (subClassPtr != IntPtr.Zero)
                {
                    subClassPtr = GetActualClass(subClassPtr);
                    string subClassSuperClassName = CHelpers.GetSuperClassName(classPtr) ?? "(null)";
                    (List<PropertyInfo> subProperties, List<MethodInfo> methods)? arrays = null;
                    if (currentDepth < maxDepth)
                    {
                        arrays = GetAllPropertiesRecursive(subClassPtr, maxDepth, currentDepth + 1);
                    }

                    properties.Add(new PropertyInfo(name, attributes, subClassSuperClassName, typeInfo, arrays?.subProperties, arrays?.methods));
                    continue;
                }
            }

            properties.Add(new PropertyInfo(name, attributes, "", typeInfo));
        }

        Marshal.FreeHGlobal(propListPtr); // Clean up unmanaged memory
        return properties;
    }

    public static IntPtr GetActualClass(IntPtr cls)
    {
        // Check if it's a KVO class
        string className = CHelpers.GetClassName(cls) ?? "(null)";
        if (className != null && className.StartsWith("NSKVONotifying_"))
        {
            return CHelpers.PropertyHelpers.class_getSuperclass(cls);
        }

        return cls;
    }

    public static string DecodeArgumentType(string typeCode)
    {
        if (string.IsNullOrWhiteSpace(typeCode))
            return "(unknown)";

        // Handle common encodings
        return typeCode switch
        {
            "v" => "void",
            "@" => "id (object)",
            ":" => "SEL (selector)",
            "#" => "Class",
            "c" => "char / BOOL",
            "C" => "unsigned char",
            "i" => "int",
            "I" => "unsigned int",
            "s" => "short",
            "S" => "unsigned short",
            "l" => "long",
            "L" => "unsigned long",
            "q" => "long long (int64_t)",
            "Q" => "unsigned long long (uint64_t)",
            "f" => "float",
            "d" => "double",
            "B" => "bool (C99 _Bool)",
            "?" => "unknown block or function pointer",
            "^v" => "void*",
            "^@" => "id*",
            "@?" => "block",
            _ when typeCode.StartsWith("T@\"") => $"object ({typeCode[3..^1]})", // T@"ClassName"
            _ => $"(unknown: {typeCode})"
        };
    }

    public static string DecodePropertyType(string attributes)
    {
        if (string.IsNullOrEmpty(attributes))
            return "(unknown)";

        var parts = attributes.Split(',');

        if (parts.Length == 0)
            return "(unknown)";

        var typeEncoding = parts[0]; // usually starts with T...

        if (typeEncoding.StartsWith("T@\"") && typeEncoding.Length > 4)
        {
            // T@"NSString" -> NSString
            return typeEncoding[3..^1];
        }
        else if (typeEncoding == "T@")
        {
            return "id (class)";
        }
        else if (typeEncoding.StartsWith("Ti"))
        {
            return "int";
        }
        else if (typeEncoding.StartsWith("TI"))
        {
            return "unsigned int";
        }
        else if (typeEncoding.StartsWith("Ts"))
        {
            return "short";
        }
        else if (typeEncoding.StartsWith("Tl"))
        {
            return "long";
        }
        else if (typeEncoding.StartsWith("Tq"))
        {
            return "long long";
        }
        else if (typeEncoding.StartsWith("TQ"))
        {
            return "unsigned long long";
        }
        else if (typeEncoding.StartsWith("Tf"))
        {
            return "float";
        }
        else if (typeEncoding.StartsWith("Td"))
        {
            return "double";
        }
        else if (typeEncoding.StartsWith("Tc"))
        {
            return "char (BOOL)";
        }
        else if (typeEncoding.StartsWith("TB"))
        {
            return "bool";
        }
        else if (typeEncoding.StartsWith("T:"))
        {
            return "SEL";
        }
        else if (typeEncoding.StartsWith("T^v"))
        {
            return "void*";
        }
        else
        {
            return $"(unknown: {typeEncoding})";
        }
    }

}

public static class CatalystWindowHelper
{

    public static UINSWindow? TryGetNSWindowFromUIWindow(UIWindow uiWindow)
    {
        // Get the NSApplication
        IntPtr nsApp = CHelpers.objc_getClass("NSApplication");
        IntPtr sharedAppSel = CHelpers.sel_registerName("sharedApplication");
        IntPtr sharedApp = CHelpers.Messaging.IntPtr_objc_msgSend(nsApp, sharedAppSel);

        // Get the delegate
        IntPtr delegateSel = CHelpers.sel_registerName("delegate");
        IntPtr appDelegate = CHelpers.Messaging.IntPtr_objc_msgSend(sharedApp, delegateSel);

        // Confirm it responds to the selector
        string selectorName = "_hostWindowForUIWindow:";
        IntPtr selector = CHelpers.sel_registerName(selectorName);
        bool canCall = CHelpers.class_respondsToSelector(CHelpers.object_getClass(appDelegate), selector);

        if (canCall)
        {
            Debug.WriteLine("✅ Delegate responds to _hostWindowForUIWindow:");
            IntPtr nsWindowProxy = CHelpers.Messaging.objc_msgSend_IntPtr(appDelegate, selector, uiWindow.Handle);

            var attachedWindowSelector = CHelpers.sel_registerName("attachedWindow");
            IntPtr nsWindow = CHelpers.Messaging.IntPtr_objc_msgSend(nsWindowProxy, attachedWindowSelector);
            if (nsWindow != IntPtr.Zero)
            {
                Console.WriteLine("🎉 Got NSWindow!");
                return new UINSWindow(nsWindow);
            }
            else
            {
                Debug.WriteLine("⚠️ Method exists, but returned nil.");
                return null;
            }
        }
        else
        {
            Debug.WriteLine("❌ Delegate does not respond to _hostWindowForUIWindow:");
        }
        return null;
    }
}