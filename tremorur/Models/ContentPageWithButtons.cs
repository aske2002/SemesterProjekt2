using shared.Models;

namespace tremorur.Models
{
    public abstract class ContentPageWithButtons : ContentPage
    {
        public readonly IButtonService _buttonService;
        public ContentPageWithButtons(IButtonService buttonService)
        {
            _buttonService = buttonService ?? throw new ArgumentNullException(nameof(buttonService));
            BindingContext = BindingContext;
            _buttonService.OnButtonClicked += OnButtonClicked;
            _buttonService.OnButtonHeld += OnButtonHeld;
            _buttonService.OnButtomMultipleClicked += OnButtonMultiClicked;
        }

        private void OnButtonMultiClicked(object? sender, ButtonMultipleClickedEventArgs message)
        {
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
            Dispatcher.Dispatch(() =>
            {
                switch (message.Button)
                {
                    case WatchButton.Cancel:
                        OnCancelButtonClicked(this, EventArgs.Empty);
                        break;
                    case WatchButton.Ok:
                        OnOKButtonClicked(this, EventArgs.Empty);
                        break;
                    case WatchButton.Up:
                        OnUpButtonClicked(this, EventArgs.Empty);
                        break;
                    case WatchButton.Down:
                        OnDownButtonClicked(this, EventArgs.Empty);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void OnButtonHeld(object? sender, ButtonHeldEventArgs message)
        {
            Dispatcher.Dispatch(() =>
            {
                switch (message.Button)
                {
                    case WatchButton.Cancel:
                        OnCancelButtonHeld(this, message.HeldMS, () => _buttonService.Hold_Handled(message.Button));
                        break;
                    case WatchButton.Ok:
                        OnOKButtonHeld(this, message.HeldMS, () => _buttonService.Hold_Handled(message.Button));
                        break;
                    case WatchButton.Up:
                        OnUpButtonHeld(this, message.HeldMS, () => _buttonService.Hold_Handled(message.Button));
                        break;
                    case WatchButton.Down:
                        OnDownButtonHeld(this, message.HeldMS, () => _buttonService.Hold_Handled(message.Button));
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

        protected virtual void OnCancelButtonHeld(object? sender, int duration, Action didHandle)
        {
        }

        protected virtual void OnOKButtonHeld(object? sender, int duration, Action didHandle)
        {
        }

        protected virtual void OnUpButtonHeld(object? sender, int duration, Action didHandle)
        {
        }

        protected virtual void OnDownButtonHeld(object? sender, int duration, Action didHandle)
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
