

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
            _buttonService.OkButtonClicked += OnOKButtonClicked;
            _buttonService.UpButtonClicked += OnUpButtonClicked;
            _buttonService.DownButtonClicked += OnDownButtonClicked;
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
