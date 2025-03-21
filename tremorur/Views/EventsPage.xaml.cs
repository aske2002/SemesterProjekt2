using System.Reflection;

namespace tremorur.Views
{
    public partial class EventsPage : ContentPage
    {
        public EventsPage(EventsViewModel viewModel)
        {
            InitializeComponent();
            var version = typeof(MauiApp).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            VersionLabel.Text = $".NET MAUI ver. {version?[..version.IndexOf('+')]}";
            BindingContext = viewModel;
            viewModel.Title = "Calendar";
            //this.SetBinding(Page.TitleProperty, static (EventsViewModel vm) => vm.Title);
            SetBinding(Page.TitleProperty, new Binding(nameof(EventsViewModel.Title)));
        }
    }
}
