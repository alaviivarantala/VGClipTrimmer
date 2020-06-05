using System.Windows.Controls;
using VGClipTrimmer.MVVM.ViewModels;

namespace VGClipTrimmer.MVVM.Views
{
    /// <summary>
    /// Interaction logic for MainViewUserControl.xaml
    /// </summary>
    public partial class MainViewUserControl : UserControl
    {
        public MainViewUserControl()
        {
            DataContext = new MainViewViewModel();
            InitializeComponent();
        }
    }
}
