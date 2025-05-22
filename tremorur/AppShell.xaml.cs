using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using tremorur.Views;

#if MACCATALYST
using UIKit;
#endif

namespace tremorur
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }
        private void RegisterRoutes()
        {
            BackgroundColor = Transparent;
            Routing.RegisterRoute("medicationPage", typeof(MedicationAlarmPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
        }
    }
}
