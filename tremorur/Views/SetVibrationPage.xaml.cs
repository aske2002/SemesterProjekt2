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

    //kode inde fra homePage
    //private int level = 1; //vibrationsstart level 1 
    //protected override void OnUpButtonClicked(object? sender, EventArgs e)
    //{
    //    if (level < 7)
    //    {
    //        level++;
    //        UpdateLevelLabel();
    //    }
    //}
    //protected override void OnDownButtonClicked(object? sender, EventArgs e)
    //{
    //    if (level > 1)
    //    {
    //        level--;
    //        UpdateLevelLabel();
    //    }
    //}

    //private void UpdateLevelLabel()
    //{
    //    LevelLabel.Text = $"Level:{level}";
    //    OnUpButtonClicked.IsEnabled = level < 7;
    //    OnDownButtonClicked.IsEnabled = level > 1;
    //}
}