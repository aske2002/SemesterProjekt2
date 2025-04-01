using CoreGraphics;
using Foundation;
using UIKit;

namespace tremorur;

[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
    public override void OnActivated(UIScene scene)
    {
        SetWindowBackgroundColor(scene);
        base.OnActivated(scene);
    }

    private void SetWindowBackgroundColor(UIScene scene)
    {
        var windowScene = scene as UIWindowScene;
        var window = windowScene?.Windows.FirstOrDefault();
        if (windowScene != null && windowScene.SizeRestrictions != null)
        {
            // Set the maximum and minimum size to 800x800
            windowScene.SizeRestrictions.MaximumSize = new CGSize(800, 800);
            windowScene.SizeRestrictions.MinimumSize = new CGSize(800, 800);


            if (windowScene.Titlebar != null)
            {
                // Hide the status bar
                windowScene.Titlebar.TitleVisibility = UITitlebarTitleVisibility.Hidden;

                if (windowScene.Titlebar.Toolbar != null)
                {
                    // Set the title bar to be hidden
                    windowScene.Titlebar.Toolbar.Visible = false;
                }
            }
        }

        if (window != null)
        {
            window.BackgroundColor = UIColor.Red;

            if (window.RootViewController != null && window.RootViewController.View != null)
            {
                window.RootViewController.View.BackgroundColor = UIColor.Red;
            }

            window.Layer.BackgroundColor = UIColor.Red.CGColor;
            window.Opaque = false;

        }
    }
}