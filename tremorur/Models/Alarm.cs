using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tremorur.Models
{
    public class Alarm
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public TimeSpan TimeSpan { get; set; }  
    }
}
