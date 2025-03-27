using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace tremorur
{
    [Register(nameof(AppDelegate))]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
