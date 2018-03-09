using Simple_Audio_Editor.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Simple_Audio_Editor.Views.Control
{
    public sealed partial class MusicPlayerSlider : UserControl
    {
        public MusicPlayerSlider()
        {
            this.InitializeComponent();
        }
        public double Maximum
        {
            get { return Maximum; }
            set { slider.Maximum = value; }
        }

    }
}
