using System.Reflection;

namespace tremorur.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            VersionLabel.Text = $".NET MAUI ver. {version?[..version.IndexOf('+')]}";
            BindingContext = viewModel;
            viewModel.Title = "Calendar";
            //this.SetBinding(Page.TitleProperty, static (EventsViewModel vm) => vm.Title);
            SetBinding(Page.TitleProperty, new Binding(nameof(HomeViewModel.Title)));
        }
    }
}
