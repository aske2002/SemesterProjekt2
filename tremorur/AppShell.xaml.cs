
namespace tremorur
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Register the routes of the detail pages
            RegisterRoutes();
            this.Appearing += AppShell_Appearing;
        }

        private void AppShell_Appearing(object? sender, EventArgs? e)
        {
            var window = Application.Current?.Windows.Count > 0 ? Application.Current.Windows[0] : null;
            if (window != null)
            {
                window.SizeChanged += WindowSizeChanged;
            }
        }



        private void WindowSizeChanged(object? sender, EventArgs? e)
        {
            // if (sender is Window window && CurrentPage != null)
            // {
            //     var widthOverflow = window.Width - CurrentPage.Bounds.Width;
            //     var heightOverflow = window.Height - CurrentPage.Bounds.Height;

            //     var idealWidth = 800 + widthOverflow;
            //     var idealHeight = 800 + heightOverflow;

            //     if (window.Width != idealWidth)
            //     {
            //         window.MaximumWidth = idealWidth;
            //         window.MinimumWidth = idealWidth;
            //     }

            //     if (window.Height != idealHeight)
            //     {
            //         window.MaximumHeight = idealHeight;
            //         window.MinimumHeight = idealHeight;
            //     }

            // }
        }

        private void RegisterRoutes()
        {
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
        }
    }
}
