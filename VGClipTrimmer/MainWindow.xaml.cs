using AdonisUI.Controls;
using VGClipTrimmer.Helpers;
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
            InitializeComponent();
            TestingMethods.PHM();
        }
    }
}
