using CoreGraphics;
using Foundation;
using UIKit;

namespace tremorur;

[Register("SceneDelegate")]
public class SceneDelegate : MauiUISceneDelegate
{
    public override void OnActivated(UIScene scene)
    {
        var windowScene = scene as UIWindowScene;
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
        base.OnActivated(scene);
    }
}