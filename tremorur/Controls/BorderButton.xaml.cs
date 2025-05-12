using shared.Messages;
using shared.Models;

namespace tremorur.Controls;
public partial class BorderButton : Grid
{

    private const string Root = "Root";
    private readonly Services.IMessenger _messenger;
    public static readonly BindableProperty ButtonBackgroundColorProperty =
            BindableProperty.Create(nameof(ButtonBackgroundColor), typeof(Color), typeof(BorderButton), Blue);
    public Color ButtonBackgroundColor
    {
        get => (Color)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    private void InnerButton_Pressed(object? sender, EventArgs e)
    {
        _messenger.SendMessage(new ButtonPressedMessage(ButtonType));
    }

    private void InnerButton_Released(object? sender, EventArgs e)
    {
        _messenger.SendMessage(new ButtonReleasedMessage(ButtonType));
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

    private void Button_Pressed(ButtonPressedMessage e)
    {
        if (e.Button == ButtonType)
        {
            Scale = 0.9;
        }
    }

    private void Button_Released(ButtonReleasedMessage e)
    {
        if (e.Button == ButtonType)
        {
            Scale = 1;
        }
    }

    public BorderButton(Services.IMessenger messenger, WatchButton buttonType)
    {
        InitializeComponent();
        ButtonType = buttonType;
        _messenger = messenger;
        _messenger.On<ButtonPressedMessage>(Button_Pressed);
        _messenger.On<ButtonReleasedMessage>(Button_Released);
        InnerButton.Text = ButtonTypes.Names[ButtonType];
    }
}
