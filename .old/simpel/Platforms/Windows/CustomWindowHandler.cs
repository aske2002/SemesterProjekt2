using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using Windows.Graphics;

public class CustomWindowHandler : Microsoft.Maui.Handlers.WindowHandler
{
    protected override void ConnectHandler(Microsoft.UI.Xaml.Window platformView)
    {
        base.ConnectHandler(platformView);

        IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(platformView);
        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
        }

        // Set Round Corners
        appWindow.Resize(new SizeInt32(400, 400)); // Adjust window size
        SetRoundedCorners(hWnd, 200); // Make the window round
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    private void SetRoundedCorners(IntPtr hWnd, int radius)
    {
        int dwmAttribute = 33; // DWMWA_WINDOW_CORNER_PREFERENCE
        int cornerPreference = 2; // DWMWCP_ROUND
        DwmSetWindowAttribute(hWnd, dwmAttribute, ref cornerPreference, sizeof(int));
    }
}
