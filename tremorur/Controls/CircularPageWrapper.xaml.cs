
namespace tremorur.Controls;
public partial class CircularPageWrapper : ContentView
{
    public CircularPageWrapper()
    {
        SizeChanged += OnSizeChanged;
        InitializeComponent();
    }

    private void OnSizeChanged(object sender, EventArgs e)
    {
        double diameter = Math.Min(Width, Height);
        MainGrid.WidthRequest = diameter;
        MainGrid.HeightRequest = diameter;
        EllipseClip.Center = new Point(diameter / 2, diameter / 2);
        EllipseClip.RadiusX = diameter / 2;
        EllipseClip.RadiusY = diameter / 2;
    }

    public View ContentInside
    {
        get => ContentHolder.Content;
        set => ContentHolder.Content = value;
    }
}
