using System.Collections.ObjectModel; //for at kunne bruge ObservableCollection
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using tremorur.Messages;

namespace tremorur.Services
{
    public class AlarmService
    {
        private ObservableCollection<Alarm> _alarms;
        private readonly IMessenger messenger;
        private List<Timer> activeTimers = new(); //gemmer alle aktive timere
        public Alarm? CurrentAlarm { get; set; }//getter og setter nuværende alarm, hvis der er nogen ellers null. Bliver brugt i AlarmTriggered i NavigationsService

        public AlarmService(IMessenger messenger)
        {
            this.messenger = messenger;
            _alarms = new ObservableCollection<Alarm>(SettingsService.Alarms);//henter eksisterende alarmer og initialiserer observableCollection
            _alarms.CollectionChanged += OnAlarmsChanged;//reagerer på ændringer i observableCollection
        }
        public Alarm CreateAlarm(TimeSpan givenTimeSpan)
        {
            var currentAlarms = SettingsService.Alarms;
            var alarm = new Alarm { Id = Guid.NewGuid().ToString(), TimeSpan = givenTimeSpan }; //opretter en alarm med et unikt Id (kaldt i Alarm.cs)
            _alarms.Add(alarm);
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
            ScheduleAlarms(); //de satte alarmer bliver kaldt
            SettingsService.Alarms = alarms; //gemmer listen ned igen via settingservice 
        }
        public Alarm? GetAlarm(string id) //Metoden tager et id som parameter (den unikke nøgle til alarmen).

        {
            return _alarms.FirstOrDefault(a => a.Id == id); //finder om der er nogen a.Id der matcher id i _alarms, og returnerer null, hvis ikke der findes et match.
        }
        public IEnumerable<Alarm> GetAlarms() 
        {
            return _alarms; //retunerer alle alarmer i ObservableCollection 
        }
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
        private void TriggerAlarm(object? state)
        {
            if (state is Alarm alarm)
            {
                Console.WriteLine($"Alarm({alarm.Id})triggered at {DateTime.Now}");

                messenger.SendMessage(new AlarmTriggeredEvent(alarm));
            }
        }
        public void ClearAlarms() //metode der sletter alle gemte alarmer
        {
            foreach (var timer in activeTimers)
            {
                timer.Dispose(); //stopper alle aktive timere
            }
            activeTimers.Clear(); //tømmer listen med aktive timere
            _alarms.Clear(); //collectionchanges tømmes og UI opdateres
            SettingsService.Alarms = new List<Alarm>(); //tømmer alarmene i settingsService

            ScheduleAlarms();//starter forfra og sætter nye timere
        }

        public void TempAlarm(TimeSpan delay, Alarm alarm)// midlertidlig alarm som udsætter alarmtidspunktet
        {
            TimeSpan triggerTime = DateTime.Now.Add(delay).TimeOfDay;//tilføjer delay til givent tidspunikt

            var tempAlarm = new Alarm { Id = Guid.NewGuid().ToString(), TimeSpan = triggerTime}; //opretter midlertidlig alarm med delay
            Timer timer = new Timer(TriggerAlarm, alarm, delay, Timeout.InfiniteTimeSpan);
            activeTimers.Add(timer);//tilføjer alarm til aktive timere
        }
    }
}
