

using System.Diagnostics;
using tremorur.Messages;

namespace tremorur.Models
{
    public abstract class ContentPageWithButtons : ContentPage
    {
        public readonly IButtonService _buttonService;
        public ContentPageWithButtons(IButtonService messenger)
        {
            _buttonService = messenger ?? throw new ArgumentNullException(nameof(messenger));
            BindingContext = BindingContext;
            _buttonService.OnButtonClicked += OnButtonClicked;
            _buttonService.OnButtonHeld += OnButtonHeld;
            _buttonService.OnButtomMultipleClicked += OnButtonMultiClicked;
        }

        private void OnButtonMultiClicked(object? sender, ButtonMultipleClickedEventArgs message)
        {
            Debug.WriteLine($"{message.Button} clicked {message.ClickCount} times");
            Dispatcher.Dispatch(() =>
            {
                switch (message.Button)
                {
                    case WatchButton.Cancel:
                        OnCancelButtonMultiClicked(this, message.ClickCount);
                        break;
                    case WatchButton.Ok:
                        OnOKButtonMultiClicked(this, message.ClickCount);
                        break;
                    case WatchButton.Up:
                        OnUpButtonMultiClicked(this, message.ClickCount);
                        break;
                    case WatchButton.Down:
                        OnDownButtonMultiClicked(this, message.ClickCount);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void OnButtonClicked(object? sender, ButtonClickedEventArgs message)
        {
            Debug.WriteLine($"{message.Button} clicked");
            Dispatcher.Dispatch(() =>
            {
                switch (message.Button)
                {
                    case WatchButton.Cancel:
                        OnCancelButtonHeld(this, 0);
                        break;
                    case WatchButton.Ok:
                        OnOKButtonHeld(this, 0);
                        break;
                    case WatchButton.Up:
                        OnUpButtonHeld(this, 0);
                        break;
                    case WatchButton.Down:
                        OnDownButtonHeld(this, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void OnButtonHeld(object? sender, ButtonHeldEventArgs message)
        {
            Debug.WriteLine($"{message.Button} held for {message.HeldMS}");
            Dispatcher.Dispatch(() =>
            {
                switch (message.Button)
                {
                    case WatchButton.Cancel:
                        OnCancelButtonHeld(this, 0);
                        break;
                    case WatchButton.Ok:
                        OnOKButtonHeld(this, 0);
                        break;
                    case WatchButton.Up:
                        OnUpButtonHeld(this, 0);
                        break;
                    case WatchButton.Down:
                        OnDownButtonHeld(this, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        protected virtual void OnCancelButtonClicked(object? sender, EventArgs e)
        {
        }

        protected virtual void OnOKButtonClicked(object? sender, EventArgs e)
        {
        }

        protected virtual void OnUpButtonClicked(object? sender, EventArgs e)
        {
        }

        protected virtual void OnDownButtonClicked(object? sender, EventArgs e)
        {
        }

        protected virtual void OnCancelButtonHeld(object? sender, int duration)
        {
        }

        protected virtual void OnOKButtonHeld(object? sender, int duration)
        {
        }

        protected virtual void OnUpButtonHeld(object? sender, int duration)
        {
        }

        protected virtual void OnDownButtonHeld(object? sender, int duration)
        {
        }

        protected virtual void OnOKButtonMultiClicked(object? sender, int clickTimes)
        {
        }

        protected virtual void OnCancelButtonMultiClicked(object? sender, int clickTimes)
        {
        }

        protected virtual void OnUpButtonMultiClicked(object? sender, int clickTimes)
        {
        }

        protected virtual void OnDownButtonMultiClicked(object? sender, int clickTimes)
        {
        }

    }
}
