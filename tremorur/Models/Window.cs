#if MACCATALYST
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

#if MACCATALYST
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
                        };
            PropertyChanged += (s, e) =>
                {
                    var uiWindow = Handler?.PlatformView as UIWindow;
                    if (uiWindow != null)
                    {

                        var nsWindow = CatalystWindowHelper.TryGetNSWindowFromUIWindow(uiWindow);
                    }
                };
#endif
            Initialize();
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
