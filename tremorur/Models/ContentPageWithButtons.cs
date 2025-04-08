

namespace tremorur.Models
{
    public abstract class ContentPageWithButtons : ContentPage
    {
        private readonly IButtonService _buttonService;
        public ContentPageWithButtons(IButtonService buttonService)
        {
            _buttonService = buttonService;
            BindingContext = BindingContext;
            InitializeButtons();
        }

        protected void InitializeButtons()
        {
            _buttonService.CancelButtonClicked += OnCancelButtonClicked;
            _buttonService.OkButtonClicked += OnButtonClicked;
            _buttonService.UpButtonClicked += OnUpButtonClicked;
            _buttonService.DownButtonClicked += OnDownButtonClicked;
        }

        protected abstract void OnCancelButtonClicked(object? sender, EventArgs e);

        protected abstract void OnButtonClicked(object? sender, EventArgs e);

        protected abstract void OnUpButtonClicked(object? sender, EventArgs e);

        protected abstract void OnDownButtonClicked(object? sender, EventArgs e);

    }
}
