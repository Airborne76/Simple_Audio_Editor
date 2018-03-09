using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple_Audio_Editor.Helpers;
using Windows.UI.Xaml.Data;

namespace Simple_Audio_Editor.Convert
{
    public class DoubleToLyricTimeString: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromMinutes((double)value).ToLyricTimeString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }
}
