using tremorur.Messages;
namespace tremorur.Services
{
    public partial class NavigationService : INavigationService
    {
        private readonly IMessenger _messenger;
        private readonly AlarmService _alarmService;
        public NavigationService(IMessenger messenger, AlarmService alarmService)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _messenger.On<AlarmTriggeredEvent>(AlarmTriggered); //når AlarmTriggered-event bliver sendt via messengeren vil AlarmTriggered metoden blive kaldt
            _alarmService = alarmService;
        }
        public async void AlarmTriggered(AlarmTriggeredEvent evt) //metode der reagerer på event og starter navigation
        {
            _alarmService.CurrentAlarm = evt.Alarm; //gemmer alarmen
            await GoToAsync("medicationPage"); //navigerer til MedicationAlarmPage
        }

        public Task GoToAsync(string route, IDictionary<string, object>? parameters = null)//metode der nagiverer til andre pages i Shell
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoToAsync)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState(route); //opretter navigations rute i shell
            _messenger.SendMessage(navigateionState); //sender messenger, så andre kan reagere på det???

            if (parameters == null) //navigere til page med eller uden parameter
            {
                return Shell.Current.GoToAsync(navigateionState);
            }
            else
            {
                return Shell.Current.GoToAsync(navigateionState, parameters);
            }
        }
        public Task GoBackAsync()
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoBackAsync)}' method is currently supported only with a Shell-enabled application.");
            }
            var navigateionState = new ShellNavigationState("..");
            _messenger.SendMessage(navigateionState);
            return Shell.Current.GoToAsync(navigateionState);
        }
    }
}
