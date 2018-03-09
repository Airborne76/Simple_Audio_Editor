using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Audio_Editor.Convert
{
    public class DoubleToWidth
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan)value).TotalMinutes;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromMinutes((double)value);
        }
    }
}
