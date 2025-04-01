// Platforms\Windows\App.xaml.cs

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace tremorur.WinUI;
public partial class App : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var mauiWindow = Application.Windows.First();

        if (mauiWindow != null)
        {
            if (mauiWindow.Handler != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mauiWindow.Handler.PlatformView);
                var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);
            }
        }
    }
}
