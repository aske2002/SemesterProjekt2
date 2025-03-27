using System.Reflection;

namespace tremorur.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
<<<<<<< Updated upstream
            var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            VersionLabel.Text = $".NET MAUI ver. {version?[..version.IndexOf('+')]}";
            BindingContext = viewModel;
            viewModel.Title = "Calendar";
            //this.SetBinding(Page.TitleProperty, static (EventsViewModel vm) => vm.Title);
            SetBinding(Page.TitleProperty, new Binding(nameof(HomeViewModel.Title)));
=======
            StartClock(); //starter uret

        }
        private async void StartClock()
        {
            while (true)
            {
                ClockLabel.Text = DateTime.Now.ToString("HH:mm"); //Opdater label med klokken. Henter den aktuelle tid i 24 timers format
                await Task.Delay(1000); //sørger for at opdateringen sker hvert sekund
            }
>>>>>>> Stashed changes
        }
    }
}
