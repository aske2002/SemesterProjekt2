#if MACCATALYST
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using UIKit;
#endif

namespace tremorur.Models
{

    public class WatchWindow : Window
    {
        public WatchWindow(Page page) : base(page)
        {
            TitleBar = new TitleBar
            {
                IsVisible = false,
            };
            Initialize();

#if MACCATALYST
            PropertyChanged += (s, e) =>
            {
                SizeChanged += (s, e) =>
                {

                    var uiWindow = Handler?.PlatformView as UIWindow;
                    if (uiWindow != null)
                    {
                        uiWindow.Center = new CoreGraphics.CGPoint(uiWindow.Frame.Width / 2, uiWindow.Frame.Height / 2);

                        var nsWindow = CatalystWindowHelper.TryGetNSWindowFromUIWindow(uiWindow);
                        if (nsWindow != null)
                        {
                            nsWindow.IsMoveableByWindowBackground = true;
                            nsWindow.StyleMask &= ~NSWindowStyle.Titled;
                            nsWindow.StyleMask &= NSWindowStyle.Borderless;
                            nsWindow.StyleMask &= ~NSWindowStyle.TexturedBackground;
                            nsWindow.BackgroundColor = NSColor.Clear;
                            nsWindow.TitlebarAppearsTransparent = true;
                            nsWindow.Opaque = false;
                        }
                    }
                    if (Shell.Current.Handler is ShellRenderer shellRenderer)
                    {
                        var tabbarView = shellRenderer.ChildViewControllers.OfType<ShellItemRenderer>().FirstOrDefault()?.View;
                        if (tabbarView != null)
                        {
                            tabbarView.BackgroundColor = UIColor.Clear;
                        }
                    }
                };
            };
#endif
        }

        public void Initialize()
        {
            MaximumHeight = 800;
            MaximumWidth = 800;
            MinimumHeight = 800;
            MinimumWidth = 800;
        }
    }
}
