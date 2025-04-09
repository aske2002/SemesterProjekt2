namespace tremorur.Services
{
    public interface INavigationService
    {
        Task GåTilSideAsync(string route);
        Task GoBackAsync();
    }
}
