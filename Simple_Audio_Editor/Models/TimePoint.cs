using Simple_Audio_Editor.Convert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Audio_Editor.Models
{
    public class TimePoint
    {
        public TimeSpan timeSpanFromStart { get; set; }
        public TimeSpan timeSpanFromEnd { get; set; }
        public double timeDouble
        {
            get
            {
                return TimeSpanSwtich.TimeSpanToDouble(timeSpanFromStart);
            }            
        }
        public double fulltime { get; set; }
    }
}
