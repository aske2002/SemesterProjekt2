using System.Collections.ObjectModel; //for at kunne bruge ObservableCollection
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using tremorur.Messages;

namespace tremorur.Services
{
    public class AlarmService
    {
        private ObservableCollection<Alarm> _alarms;

        public AlarmService() 
        {
            _alarms = new ObservableCollection<Alarm>(SettingsService.Alarms); //henter eksisterende alarmer og initialiserer observableCollection

            _alarms.CollectionChanged += OnAlarmsChanged; //reagerer på ændringer i observableCollection
        }
        public Alarm CreateAlarm(TimeSpan givenTimeSpan)
        {

            var currentAlarms = SettingsService.Alarms;
            var alarm = new Alarm { Id = Guid.NewGuid().ToString(), TimeSpan = givenTimeSpan }; //opretter en alarm med et unikt Id (kaldt i Alarm.cs)
            _alarms.Add(alarm);
            SettingsService.Alarms = _alarms.ToList(); //opdaterer SettingService med nye alarmer
            return alarm;
        }
        private void OnAlarmsChanged(object sender, NotifyCollectionChangedEventArgs e) //funktion til at håndterer ændringer i ObservableCollection
        {
            var alarms = SettingsService.Alarms.ToList();
            if (e.NewItems != null)
            {
                foreach (Alarm newAlarm in e.NewItems)
                {
                    alarms.Add(newAlarm); //tilføjer nye alarmer
                }
            }
            SettingsService.Alarms = alarms; //gemmer listen ned igen via settingservice 
        }

        public Alarm? GetAlarm(string id) //Metoden tager et id som parameter (den unikke nøgle til alarmen).

        {
            string timeString = Preferences.Get(id + "_time", ""); //forsøger at hente den tidligere gemte alarm fra preferences, hvis der er en gemt, returneres en string som 00:30:00 hvis ikke der findes en gemt alarm, bliver en tom string returneret
            return string.IsNullOrEmpty(timeString) ? null : new Alarm { Id = id, TimeSpan = TimeSpan.Parse(timeString) }; //returnerer null hvis timeString er tom - hvis den har en værdi oprettes en ny alarm med ...
        }
        public IEnumerable<Alarm> GetAlarms() 
        {
            return _alarms; //retunerer alle alarmer i ObservableCollection 
        }

        private List<Timer> activeTimers = new(); //gemmer alle aktive timere

        public void ScheduleAlarms() //har et alarm tidspunkt og undersøger om tidspunktet er overskredet - hvis det ikke er, kører alarmen på tidspunktet, hvis tidspunktet er overskredet, bliver alarmet sat til dagen efter
        {
            foreach (var alarm in SettingsService.Alarms)
            {
                TimeSpan delay = alarm.TimeSpan - DateTime.Now.TimeOfDay;
                if (delay < TimeSpan.Zero)
                    delay += TimeSpan.FromDays(1);
                Timer timer = new Timer(TriggerAlarm, alarm, delay, Timeout.InfiniteTimeSpan);
                activeTimers.Add(timer);
            }

        }
        private readonly Messenger messenger;
        public AlarmService(Messenger messenger)
        {
            this.messenger = messenger;
            _alarms=new ObservableCollection<Alarm>(SettingsService.Alarms);
            _alarms.CollectionChanged += OnAlarmsChanged;
        }
        private void TriggerAlarm(object? state)
        {
            if (state is Alarm alarm)
            {
                Console.WriteLine($"Alarm({alarm.Id})triggered at {DateTime.Now}");

                messenger.SendMessage(new AlarmTriggered(alarm));
            }
        }
    }
}
