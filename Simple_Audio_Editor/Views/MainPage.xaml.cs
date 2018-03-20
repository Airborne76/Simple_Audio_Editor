using System;
using System.Diagnostics;
using Simple_Audio_Editor.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Simple_Audio_Editor.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
            ViewModel.Initialize();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            Initialize();
        }
        private void Initialize()
        {
            slider.AddHandler(PointerPressedEvent, new PointerEventHandler(ViewModel.silder_PointerPressed), true);
            slider.AddHandler(PointerReleasedEvent, new PointerEventHandler(ViewModel.silder_Pointerreleased), true);
            processSilder.AddHandler(PointerPressedEvent, new PointerEventHandler(ViewModel.processSilder_PointerPressed), true);
            processSilder.AddHandler(PointerReleasedEvent, new PointerEventHandler(ViewModel.processSilder_Pointerreleased), true);


        }

        private void Page_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            DragIcon.Visibility = Visibility.Visible;
        }

        private void Page_DragLeave(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            DragIcon.Visibility = Visibility.Collapsed;
        }

        private void Page_Drop(object sender, DragEventArgs e)
        {
            DragIcon.Visibility = Visibility.Collapsed;
        }

    }
}
