using System.Windows;

namespace GameHighlightClipper.Helpers
{
    public static class Utilities
    {
        public static void ShutdownApp()
        {
            Application.Current.Shutdown();
        }
    }
}
