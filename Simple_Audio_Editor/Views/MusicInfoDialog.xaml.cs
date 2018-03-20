using Simple_Audio_Editor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Simple_Audio_Editor.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MusicInfoDialog : ContentDialog
    {
        public MusicInfoDialog(MusicInfo musicInfo)
        {
            this.MusicInfo = musicInfo;
            this.InitializeComponent();
        }
        public BitmapImage bitmapImage { get { return MusicInfo.bitmapImage; } }
        public MusicProperties musicProperties { get { return MusicInfo.musicProperties; } }
        public MusicInfo MusicInfo { get; set; }

    }
}
