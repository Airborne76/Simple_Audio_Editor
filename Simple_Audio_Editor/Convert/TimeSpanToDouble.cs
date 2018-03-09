using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Simple_Audio_Editor.Convert
{
    public class TimeSpanToDouble: IValueConverter
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
    public class TimeSpanSwtich
    {
        public static double TimeSpanToDouble(TimeSpan value)
        {
            return value.TotalMinutes;
        }
        public static TimeSpan DoubleToTimeSpan(double value)
        {
            return TimeSpan.FromMinutes(value);
        }
    }
}
