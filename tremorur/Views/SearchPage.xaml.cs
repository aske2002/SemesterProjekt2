namespace tremorur.Views
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage(MedicationAlarmViewModel viewModel)
        {
            InitializeComponent();
            viewModel.Title = "Search";
            BindingContext = viewModel;
        }
    }
}
