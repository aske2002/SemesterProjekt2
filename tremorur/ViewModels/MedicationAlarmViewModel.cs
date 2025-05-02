using System.Diagnostics;

namespace tremorur.ViewModels
{
    

    public partial class MedicationAlarmViewModel(IDialogService dialogService, INavigationService navigationService) : BaseViewModel(dialogService, navigationService)
    {
        private Alarm? alarm { get; set; }
        Alarm? AlarmToShow
        {
            get
            {
                return alarm;
            }
            set
            {
                Debug.WriteLine("Alarm sat!");
                alarm = value;
            }
        }

        [RelayCommand]
        private Task LoginAsync() => NavigationService.GoToAsync("//home");
    }
}
