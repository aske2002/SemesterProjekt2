namespace tremorur
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register the routes of the detail pages
            RegisterRoutes();
            this.BackgroundColor = Colors.Transparent;
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("newevent", typeof(NewEventPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Current.GoToAsync("//login");
        }
    }
}
