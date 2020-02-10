using AdonisUI;
using AdonisUI.Controls;
using System;
using System.Windows;

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
        }

        private bool _isDark;
        private void ChangeTheme(object sender, RoutedEventArgs e)
        {
            ResourceLocator.SetColorScheme(Application.Current.Resources, _isDark ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);

            _isDark = !_isDark;
        }
        private void ShutdownApp()
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void PHM()
        {
            string clips = "C:/clips/";
            //await Task.Run(() => TestBatch());
            //var r = FFmpeg.CutVideo(clips + "APEX2.mp4", clips + "outAPEX2.mp4", new TimeSpan(0, 0, 55), new TimeSpan(0, 3, 55));
            TestingMethods.CreateImages(clips);
            ShutdownApp();
        }
    }
}
