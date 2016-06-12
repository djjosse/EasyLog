using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyLog
{
    public class Message
    {
        public string Text { get; set; }
        public eCategory Category { get; set; }
        public string Method { get; set; }
        public string Module { get; set; }
        public DateTime Time { get; set; }
    }
}
