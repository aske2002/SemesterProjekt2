namespace tremorur.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        public HomeViewModel(IDialogService dialogService, INavigationService navigationService) : base(dialogService, navigationService)
        {
            Level = 0;
        }
        [ObservableProperty]
        private int level;
        
        public string LevelText=>$"Level:{level}";

        partial void OnLevelChanged(int oldValue,int newValue)
        {
            OnPropertyChanged(nameof(LevelText));
        }

        [RelayCommand]
        private Task SetAlarmAsync() => NavigationService.GoToAsync("//SetAlarm");
    }
}
