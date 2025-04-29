using System.Collections.ObjectModel;
using tremorur.Models;
using tremorur.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace tremorur.ViewModels
{
    public partial class AlarmListViewModel : ObservableObject
    {
        private readonly AlarmService _alarmService;

        [ObservableProperty]
        private ObservableCollection<Alarm> alarms;

        [ObservableProperty]
        private Alarm? selectedAlarm;

        public AlarmListViewModel(AlarmService alarmService)
        {
            _alarmService = alarmService;
            Alarms = new ObservableCollection<Alarm>(_alarmService.GetAlarms());
        }

        public void DeleteSelectedAlarm()
        {
            if (SelectedAlarm != null)
            {
                _alarmService.DeleteAlarm(SelectedAlarm.Id);
                Alarms.Remove(SelectedAlarm);
                SelectedAlarm = null;
            }
        }

        public void SelectNextAlarm()
        {
            if (Alarms.Count == 0) return;

            int currentIndex = SelectedAlarm != null ? Alarms.IndexOf(SelectedAlarm) : -1;
            currentIndex = (currentIndex + 1) % Alarms.Count;
            SelectedAlarm = Alarms[currentIndex];
        }

        public void SelectPreviousAlarm()
        {
            if (Alarms.Count == 0) return;

            int currentIndex = SelectedAlarm != null ? Alarms.IndexOf(SelectedAlarm) : 0;
            currentIndex = (currentIndex <= 0) ? Alarms.Count - 1 : currentIndex - 1;
            SelectedAlarm = Alarms[currentIndex];
        }
    }
}
