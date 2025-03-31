// Platforms\Windows\App.xaml.cs

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

public partial class App : MauiWinUIApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        var mauiWindow = Application.Current.Windows.First();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(mauiWindow.Handler.PlatformView);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        // Set initial size if desired
        appWindow.Resize(new SizeInt32(800, 600));

        // Lock aspect ratio to 4:3 (example)
        const double aspectRatio = 4.0 / 3.0;
        appWindow.Changed += (s, e) =>
        {
            var size = appWindow.Size;
            var newHeight = (int)(size.Width / aspectRatio);
            appWindow.Resize(new SizeInt32(size.Width, newHeight));
        };
    }
}
