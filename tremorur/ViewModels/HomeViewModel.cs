namespace tremorur.ViewModels
{
    public partial class HomeViewModel(IDialogService dialogService, INavigationService navigationService) : BaseViewModel(dialogService, navigationService)
    {
        [RelayCommand]
        private Task SetAlarmAsync() => NavigationService.GoToAsync("//SetAlarm");
    }
}
