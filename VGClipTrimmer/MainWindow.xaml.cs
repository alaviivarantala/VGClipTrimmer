using AdonisUI;
using AdonisUI.Controls;
using System.Windows;
using VGClipTrimmer.MVVM.ViewModels;

namespace VGClipTrimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        public MainWindow()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }
    }
}
