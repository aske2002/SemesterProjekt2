
using System.Diagnostics;
using System.Reflection.Metadata;
using Foundation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using UIKit;

namespace tremorur
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();

#if MACCATALYST
            HandlerChanged += (s, e) =>
            {
                if (Handler is ShellRenderer shellRenderer)
                {
                    var tabbarView = shellRenderer.ChildViewControllers.OfType<ShellItemRenderer>().FirstOrDefault()?.View;
                    if (tabbarView != null)
                    {
                        tabbarView.BackgroundColor = UIColor.Clear;
                    }
                }
            };

#endif

        }
        private void RegisterRoutes()
        {
            BackgroundColor = Transparent;
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
        }
    }
}
