namespace tremorur.ViewModels
{
    public partial class SetAlarmViewModel(IDialogService dialogService, INavigationService navigationService) : BaseViewModel(dialogService, navigationService)
    {
        [RelayCommand]
        private Task LoginAsync() => NavigationService.GåTilSideAsync("//home");
    }
}
