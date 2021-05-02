using GameHighlightClipper.MVVM.Models.Interfaces;
using GameHighlightClipper.MVVM.Models.Services;
using NLog;
using System;
using System.Windows;

namespace GameHighlightClipper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private INLogLogger logger = new NLogLogger();

        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
        }

        private void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            logger.LogTrace(e);
            if (e.InnerException != null)
            {
                logger.LogTrace(e.InnerException);
            }
        }
    }
}