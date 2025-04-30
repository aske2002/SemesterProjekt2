using tremorur.Messages;
namespace tremorur.Services
{
    public partial class NavigationService : INavigationService
    {
        private readonly IMessenger _messenger;
        public NavigationService(IMessenger messenger)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _messenger.On<AlarmTriggered>(AlarmTriggered); //når AlarmTriggered-event bliver sendt via messengeren vil AlarmTriggered metoden blive kaldt
        }
        public async void AlarmTriggered(AlarmTriggered evt)
        {
            var triggered = new Dictionary<string, object>() //opretter et Dictionary når AlarmTriggered-event modtages
            {
                {"alarm",evt.Alarm}//"alarm" er navnet på parameteren som sendes til medicationPage
            };
            await GoToAsync("medicationPage",triggered); //går til medicationPage, når event er triggered
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
