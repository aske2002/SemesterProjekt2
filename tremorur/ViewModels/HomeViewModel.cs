using tremorur.Messages;

namespace tremorur.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IBluetoothStateManager _bluetoothStateManager;
        public HomeViewModel(IDialogService dialogService, INavigationService navigationService, IBluetoothStateManager bluetoothStateManager) : base(dialogService, navigationService)
        {
            Level = 0;
            _bluetoothStateManager = bluetoothStateManager;
            _bluetoothStateManager.ConnectionStateChanged += OnConnectionStateChanged;
        }

        private void OnConnectionStateChanged(object? sender, bool e)
        {
            OnPropertyChanged(nameof(ConnectionStatusText));
            OnPropertyChanged(nameof(ConnectionStatusBackgroundColor));
        }
        public Color ConnectionStatusBackgroundColor => _bluetoothStateManager.IsConnected ? Green : Red;
        public string ConnectionStatusText => _bluetoothStateManager.IsConnected ? "Watch connected" : "Watch not connected";

        [ObservableProperty]
        private int level;

        public string LevelText => $"Level:{Level}";

        partial void OnLevelChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(LevelText));
        }

        [RelayCommand]
        private Task SetAlarmAsync() => NavigationService.GoToAsync("//SetAlarm");
    }
}
