using System;

using Simple_Audio_Editor.ViewModels;

using Windows.UI.Xaml.Controls;

namespace Simple_Audio_Editor.Views
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(shellFrame);
        }
    }
}
