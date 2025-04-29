using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tremorur.Messages
{
    public class AlarmTriggered
    {
        public Alarm Alarm { get; set; }
        public AlarmTriggered(Alarm alarm)
        {
            Alarm = alarm;
        }
    }
}
