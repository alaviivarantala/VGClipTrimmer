using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models.Services;
using Unity;

namespace GameHighlightClipper.MVVM.ViewModels
{
    public class ViewModelLocator
    {
        private IUnityContainer _container;

        public ViewModelLocator()
        {
            _container = new UnityContainer();
            _container.RegisterType<IVideoProcessingService, VideoProcessingService>();
            _container.RegisterType<INLogLogger, NLogLogger>();
        }

        public MainViewViewModel MainView
        {
            get { return _container.Resolve<MainViewViewModel>(); }
        }
        public MainWindowViewModel MainWindow
        {
            get { return _container.Resolve<MainWindowViewModel>(); }
        }
    }
}
