using tremorur.Messages;

namespace tremorur.Services
{
    public partial class NavigationService : INavigationService
    {
        private readonly IMessenger _messenger;
        public NavigationService(IMessenger messenger)
        {
            _messenger = messenger;
        }

        private void OnUpdateApplication(Type[]? types)
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(OnUpdateApplication)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState("..");
            _messenger.SendMessage(navigateionState);
            Shell.Current.GoToAsync(navigateionState);
        }

        public Task GoToAsync(string route, IDictionary<string, object>? parameters = null)
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoToAsync)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState(route);
            _messenger.SendMessage(navigateionState);
            return Shell.Current.GoToAsync(navigateionState, parameters);
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
