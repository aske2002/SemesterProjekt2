using System.Globalization;
using Microsoft.Maui.HotReload;
using tremorur.Messages;
using tremorur.Utils;

namespace tremorur.Controls;
public partial class BorderButton : Grid
{

    private const string Root = "Root";
    private readonly Services.IMessenger messenger;
    public static readonly BindableProperty ButtonBackgroundColorProperty =
            BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(BorderButton), Blue);
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    private void InnerButton_Pressed(object? sender, EventArgs e)
    {
        messenger.SendMessage(new ButtonPressedMessage(ButtonType));
    }

    private void InnerButton_Released(object? sender, EventArgs e)
    {
        messenger.SendMessage(new ButtonReleasedMessage(ButtonType));
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
        InitializeComponent();
        ButtonType = buttonType;
        this.messenger = messenger;
        InnerButton.Text = ButtonTypes.Names[ButtonType];
    }
}
