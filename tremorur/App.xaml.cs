using System.Diagnostics;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using tremorur.Messages;

namespace tremorur
{
    public partial class App : Application
    {
        private List<BorderButton>? _watchButtons = null;
        private Services.IMessenger _messenger;
        private IButtonService _buttonService;
        private INavigationService _navigationService;
        private Grid? _grid = null;
        private AbsoluteLayout? _absoluteLayout = null;

        public App(Services.IMessenger messenger, IButtonService buttonService, INavigationService navigationService)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _buttonService = buttonService ?? throw new ArgumentNullException(nameof(buttonService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _buttonService.OnButtomMultipleClicked += NavigateToBluetoothConnectPage;
            InitializeComponent();
        }

        private void NavigateToBluetoothConnectPage(object? sender, ButtonMultipleClickedEventArgs e)
        {
            if (e.Button == WatchButton.Ok && e.ClickCount == 4)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _navigationService.GoToAsync("///bluetoothConnect");
                });

            }
        }
        protected override void OnResume()
        {
            base.OnResume();
        }

        public void AbsoluteLayoutLoaded(object? sender, EventArgs? e)
        {
            if (sender is AbsoluteLayout absoluteLayout)
            {
                _absoluteLayout = absoluteLayout;
                _absoluteLayout.WidthRequest = 800;
                _absoluteLayout.HeightRequest = 800;
                _watchButtons = ButtonTypes.CreateButtons(_messenger);
                _watchButtons.ForEach(absoluteLayout.Children.Add);
                _watchButtons.ForEach(button => button.SizeChanged += SizeChanged);
                _absoluteLayout.SizeChanged += SizeChanged;
                SizeChanged(sender, e);
            }
        }

        public void GridLoaded(object? sender, EventArgs? e)
        {
            if (sender is Grid grid)
            {
                _grid = grid;
                _grid.SizeChanged += SizeChanged;
                SizeChanged(sender, e);
            }
        }

        [Development.HotReload.OnHotReload]
        private void SizeChanged(object? sender, EventArgs? e)
        {
            if (_absoluteLayout == null || this._grid == null)
                return;

            double smallestDimension = Math.Min(_absoluteLayout.Width, _absoluteLayout.Height);
            double diameter = smallestDimension * 0.8; // 80% of the smallest dimension
            AbsoluteLayout.SetLayoutBounds(_grid, new Rect(0.5, 0.5, -1, -1)); // Center the grid in the AbsoluteLayout
            AbsoluteLayout.SetLayoutFlags(_grid, AbsoluteLayoutFlags.PositionProportional);
            _grid.WidthRequest = diameter;
            _grid.HeightRequest = diameter;

            var radius = diameter / 2;
            if (_grid.Clip is EllipseGeometry ellipse)
            {
                ellipse.RadiusX = radius;
                ellipse.RadiusY = radius;
                ellipse.Center = new Point(radius, radius);
            }

            if (_watchButtons != null)
            {
                // 4 angles, each 90° apart, starting at 45° offset
                foreach (var button in _watchButtons)
                {
                    button.AnchorX = 0.5;
                    button.AnchorY = 0.5;
                    double angleDeg = button.ButtonPosition;
                    double angleRad = angleDeg * Math.PI / 180;

                    double buttonRadius = radius + button.DesiredSize.Height;;

                    // Get x/y position in absolute layout terms (0.0 to 1.0)
                    double x = (_absoluteLayout.Width / 2) + (buttonRadius * Math.Cos(angleRad));
                    double y = (_absoluteLayout.Height / 2) + (buttonRadius * Math.Sin(angleRad));

                    button.Rotation = angleDeg + 90; // Rotate the button to face outward

                    AbsoluteLayout.SetLayoutBounds(button, new Rect(x / _absoluteLayout.Width, y / _absoluteLayout.Height, button.Width, button.Height));
                    AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.PositionProportional);
                }
            }
        }



        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new WatchWindow(new AppShell());
        }
    }
}
