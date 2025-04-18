using Microsoft.Extensions.Logging;

namespace tremorur.Views;

public partial class SetVibrationsPage : ContentPageWithButtons
{
    private readonly INavigationService navigationService;
    public SetVibrationsPage(IButtonService buttonService, INavigationService navigationService) : base(buttonService)
    {
        InitializeComponent();
        this.navigationService = navigationService;
    }
}