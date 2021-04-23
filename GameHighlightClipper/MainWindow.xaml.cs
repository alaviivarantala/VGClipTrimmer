using AdonisUI.Controls;
using GameHighlightClipper.Helpers;
using GameHighlightClipper.MVVM.ViewModels;

namespace GameHighlightClipper
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
