using tremorur.Models;
using tremorur.ViewModels;

namespace tremorur.Views;

public partial class AlarmListPage : ContentPageWithButtons
{
    private readonly AlarmListViewModel _viewModel;
    private bool isPressed = false;
    private DateTime pressedTime;

    public AlarmListPage(AlarmListViewModel viewModel, IButtonService buttonService)
        : base(buttonService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    protected override void OnCancelButtonHeld(object? sender, int ms, Action didHandle)
    {
        if (ms >= 4000)
        {
            didHandle();
            if (_viewModel.SelectedAlarm != null)
            {
                _viewModel.DeleteSelectedAlarm();
            }
        }
    }
    

    private async void Cancel(object sender, EventArgs e)
{
    if (isPressed)
    {
        isPressed = false;
        var duration = (DateTime.Now - pressedTime).TotalMilliseconds;

        if (duration >= 4000)
        {
            if (_viewModel.SelectedAlarm != null)
            {
                _viewModel.DeleteSelectedAlarm();
                await Application.Current.MainPage.DisplayAlert("Success", "Alarm slettet", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Info", "Ingen alarm valgt", "OK");
            }
        }
    }
}
}





