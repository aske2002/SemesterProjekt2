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
        public override UIWindow? Window { get; set; }

        public override void WillEnterForeground(UIApplication application)
        {
            base.WillEnterForeground(application);
            MakeWindowTransparent();
        }
        public void MakeWindowTransparent()
        {
            if (Window != null)
            {
                Window.BackgroundColor = UIColor.Clear;
                Window.Opaque = false;

                if (Window.RootViewController != null && Window.RootViewController.View != null)
                {
                    Window.RootViewController.View.BackgroundColor = UIColor.Clear;
                }
            }
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            MakeWindowTransparent();
            return base.FinishedLaunching(application, launchOptions);
        }
    }
}
