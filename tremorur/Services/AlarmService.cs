using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace tremorur.Services
{
    public class AlarmService
    {
        public Alarm CreateAlarm(TimeSpan givenTimeSpan)
        {
            var alarm = new Alarm { Id = Guid.NewGuid().ToString(), TimeSpan = givenTimeSpan }; //opretter en alarm med et unikt Id (kaldt i Alarm.cs)
            Preferences.Set(alarm.Id + "_time", alarm.TimeSpan.ToString()); //Gemmer TimeSpan (tidspunkt i timer og minutter) som en string i preferences
            return alarm; //retunerer alarm direkte, uden at hente fra preferences 
        }
        public Alarm? GetAlarm(string id) //Metoden tager et id som parameter (den unikke nøgle til alarmen).

        {
            string timeString = Preferences.Get(id + "_time", ""); //forsøger at hente den tidligere gemte alarm fra preferences, hvis der er en gemt, returneres en string som 00:30:00 hvis ikke der findes en gemt alarm, bliver en tom string returneret
            return string.IsNullOrEmpty(timeString) ? null : new Alarm { Id = id, TimeSpan = TimeSpan.Parse(timeString) }; //returnerer null hvis timeString er tom - hvis den har en værdi oprettes en ny alarm med ...
        }
    }
}
