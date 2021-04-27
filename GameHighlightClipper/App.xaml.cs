using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models.Services;
using System;
using System.Windows;
using Unity;

namespace GameHighlightClipper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        INLogLogger logger;

        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            IUnityContainer container = new UnityContainer();
            container.RegisterType<IVideoProcessingService, VideoProcessingService>();
            container.RegisterType<INLogLogger, NLogLogger>();

            logger = container.Resolve<INLogLogger>();

            try
            {
                throw new Exception("1");
            }
            catch (Exception e)
            {
                Console.WriteLine("Catch clause caught : {0} \n", e.Message);
            }

            throw new Exception("2");
        }

        void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            logger.Trace(e);
            if (e.InnerException != null)
            {
                logger.Trace(e.InnerException);
            }
        }
    }
}
