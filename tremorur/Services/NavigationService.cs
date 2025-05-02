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
        public Task GoBackAsync() //bliver denne metode brugt nogen steder???!!!
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoBackAsync)}' method is currently supported only with a Shell-enabled application.");
            }
            var navigateionState = new ShellNavigationState("..");
            _messenger.SendMessage(navigateionState);
            return Shell.Current.GoToAsync(navigateionState);
        }

        public async void AlarmTriggered(AlarmTriggeredEvent evt) //metode der reagerer på event og starter navigation - modtager event-objekt, der indeholder den alarm der bliver trigget
        {
            _alarmService.CurrentAlarm = evt.Alarm; //gemmer den triggede alarm i AlarmService, så den kan hentes i UI senere
            Shell.Current?.Dispatcher.Dispatch(async () => //hvis shell og UI er klar, udføres koden i MAUI
            {
                var dict = new Dictionary<string, object> 
                {
                    { "alarmId", evt.Alarm.Id } //sender alarmId med som parameter
                };

                try
                {
                    await GoToAsync("///medicationPage", dict); //navigerer til medicationAlarmPage med alarm-ID som parameter
                }
                catch (Exception ex) { } //håndterer eventuelle fejl
            });
        }
    }
}
