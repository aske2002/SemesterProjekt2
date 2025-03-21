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
        public Task GoToAsync(string route)
        {
            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoToAsync)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState(route);
            _messenger.Send(navigateionState);
            return Shell.Current.GoToAsync(navigateionState);
        }

        public Task GoBackAsync()
        {


            if (Shell.Current is null)
            {
                throw new NotSupportedException($"Navigation with the '{nameof(GoBackAsync)}' method is currently supported only with a Shell-enabled application.");
            }

            var navigateionState = new ShellNavigationState("..");
            _messenger.Send(navigateionState);
            return Shell.Current.GoToAsync(navigateionState);
        }
    }
}
