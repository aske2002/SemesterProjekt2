using System.Globalization;
using Microsoft.Maui.HotReload;
using tremorur.Messages;

namespace tremorur.Controls;
public partial class BorderButton : Grid
{
    private const string Root = "Root";
    public static readonly BindableProperty ButtonBackgroundColorProperty =
            BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(BorderButton), Blue);
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    // Optional: Expose Clicked event
    public event EventHandler? Clicked;

    private void InnerButton_Clicked(object sender, EventArgs e)
    {
        Clicked?.Invoke(this, e); // Bubble up the click event
    }

    public WatchButton ButtonType;

    public string ButtonText
    {
        get => ButtonTypes.Names[ButtonType];
    }
    public double ButtonPosition
    {
        get => ButtonTypes.Positions[ButtonType];
    }
    public BorderButton(Services.IMessenger messenger, WatchButton buttonType)
    {
        ButtonType = buttonType;
        InitializeComponent();
    }
}
