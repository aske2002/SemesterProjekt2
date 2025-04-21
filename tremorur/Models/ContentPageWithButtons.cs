

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
        }

        private void OnButtonClicked(object? sender, ButtonClickedEventArgs message)
        {
            MainThread.BeginInvokeOnMainThread(() =>
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

    }
}
