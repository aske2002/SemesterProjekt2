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
            _messenger.SendBegivenhed(navigateionState);
            Shell.Current.GoToAsync(navigateionState);
        }

        public Task GåTilSideAsync(string route)
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GåTilSideAsync)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState(route);
            _messenger.SendBegivenhed(navigateionState);
            return Shell.Current.GoToAsync(navigateionState);
        }

        public Task GoBackAsync()
        {


            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoBackAsync)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState("..");
            _messenger.SendBegivenhed(navigateionState);
            return Shell.Current.GoToAsync(navigateionState);
        }
    }
}
