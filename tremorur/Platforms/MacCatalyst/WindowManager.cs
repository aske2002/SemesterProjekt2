using UIKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using AppKit;

namespace tremorur.Platforms.MacCatalyst
{
    public static class WindowManager
    {
        public static void ConfigureWindow(UIWindowScene scene)
        {
            var window = scene.KeyWindow;
            if (window != null)
            {
                // Set fixed size
                var fixedSize = new CGSize(800, 600);
                window.Frame = new CGRect(window.Frame.X, window.Frame.Y, fixedSize.Width, fixedSize.Height);

                // Get native NSWindow and disable resizing
                var nsWindow = GetNSWindow(window);
            }
        }

        private static UIWindow GetNSWindow(UIWindow uiWindow)
        {
            var nsWindowPtr = uiWindow.Handle;
            return Runtime.GetNSObject<UIWindow>(nsWindowPtr);
        }
    }
}
