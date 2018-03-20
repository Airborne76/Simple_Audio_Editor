using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace Simple_Audio_Editor.Models
{
    public class MusicInfo
    {
        //public StorageItemThumbnail storageItemThumbnail { get; set; }
        public BitmapImage bitmapImage { get; set; }
        public MusicProperties musicProperties { get; set; }
    }
}
