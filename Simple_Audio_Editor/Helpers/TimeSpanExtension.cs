using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Audio_Editor.Helpers
{
    public static class TimeSpanExtension
    {
        public static string ToLyricTimeString(this TimeSpan time)
        {
            return $"{time.Minutes:D2}:{time.Seconds:D2}.{time.Milliseconds.ToString("D3").Remove(2)}";

        }
    }
}
