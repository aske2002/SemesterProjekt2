using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tremorur.Messages
{
    public class AlarmTriggeredEvent
    {
        public Alarm Alarm { get; set; }
        public AlarmTriggeredEvent(Alarm alarm)
        {
            Alarm = alarm;
        }
    }
}
