using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using tremorur.Messages;

namespace tremorur
{
    public partial class App : Application
    {
        private List<BorderButton>? _watchButtons = null;
        private Services.IMessenger _messenger;

        private Grid? _grid = null;
        private AbsoluteLayout? _absoluteLayout = null;

        public App(Services.IMessenger messenger)
        {
            _messenger = messenger;
            InitializeComponent();
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
                _watchButtons = ButtonTypes.CreateButtons(_messenger);
                _watchButtons.ForEach(absoluteLayout.Children.Add);
                _absoluteLayout.SizeChanged += SizeChanged;
            }
        }

        public void GridLoaded(object? sender, EventArgs? e)
        {
            if (sender is Grid grid)
            {
                _grid = grid;
                _grid.SizeChanged += SizeChanged;
            }
        }

        [Development.HotReload.OnHotReload]
        private void SizeChanged(object? sender, EventArgs? e)
        {
            if (_absoluteLayout == null || this._grid == null)
                return;

            double diameter = Math.Min(_absoluteLayout.Width, _absoluteLayout.Height);

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
                    double angleDeg = button.ButtonPosition;
                    double angleRad = angleDeg * Math.PI / 180;

                    double buttonRadius = radius + (button.Height / 2);

                    // Get x/y position in absolute layout terms (0.0 to 1.0)
                    double x = 0.5 + buttonRadius * Math.Cos(angleRad) / diameter;
                    double y = 0.5 + buttonRadius * Math.Sin(angleRad) / diameter;

                    button.Rotation = angleDeg + 90; // Point outward from circle

                    AbsoluteLayout.SetLayoutBounds(button, new Rect(x, y, -1, -1));
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
