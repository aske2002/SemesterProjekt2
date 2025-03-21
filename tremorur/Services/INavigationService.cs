namespace tremorur.Services
{
    public interface INavigationService
    {
        Task GoToAsync(string route);
        Task GoBackAsync();
    }
}
