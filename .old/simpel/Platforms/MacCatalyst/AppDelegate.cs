using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using ScreenCaptureKit;
using UIKit;


namespace Tremorur;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
	{
		var result = base.FinishedLaunching(application, launchOptions);
		var window = UIApplication.SharedApplication.ConnectedScenes.OfType<UIWindowScene>()
			.SelectMany(x => x.Windows)
			.FirstOrDefault(x => x.IsKeyWindow);
		var windowScene = window?.WindowScene;

		if (window == null || windowScene == null)
		{
			return result;
		}


		// Remove title bar and set transparent background
		if (windowScene.SizeRestrictions != null)
		{
			windowScene.SizeRestrictions.MinimumSize = new CGSize(400, 400);
			windowScene.SizeRestrictions.MaximumSize = new CGSize(400, 400);
		}

		if (windowScene.Titlebar != null)
		{
			windowScene.Titlebar.TitleVisibility = UITitlebarTitleVisibility.Hidden;
			
		}
		
		window.BackgroundColor = UIColor.Clear;
		window.Layer.CornerRadius = 200;
		window.MakeKeyAndVisible();
		window.Opaque = true;

		return result;
	}
}
