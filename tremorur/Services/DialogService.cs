namespace tremorur.Services
{
    public partial class DialogService : IDialogService
    {
        public Task DisplayAlertAsync(string title, string message, string cancel)
        {
            var page = Application.Current?.Windows?[0]?.Page;

            return page switch
            {
                Shell => Shell.Current.DisplayAlert(title, message, cancel),
                not null => page.DisplayAlert(title, message, cancel),
                _ => throw new InvalidOperationException("Window's Page cannot be null.")
            };
        }

        public Task DisplayAlertAsync(string title, string message, string accept, string cancel)
        {
            var page = Application.Current?.Windows?[0]?.Page;

            return page switch
            {
                Shell => Shell.Current.DisplayAlert(title, message, accept, cancel),
                not null => page.DisplayAlert(title, message, accept, cancel),
                _ => throw new InvalidOperationException("Window's Page cannot be null.")
            };
        }

        public Task<string> DisplayPromptAsync(string title, string message, string accept, string cancel)
        {
            var page = Application.Current?.Windows?[0]?.Page;

            return page switch
            {
                Shell => Shell.Current.DisplayPromptAsync(title, message, cancel),
                not null => page.DisplayPromptAsync(title, message, cancel),
                _ => throw new InvalidOperationException("Window's Page cannot be null.")
            };
        }
    }
}
