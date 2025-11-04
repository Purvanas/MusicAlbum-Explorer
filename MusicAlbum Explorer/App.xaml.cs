using Microsoft.Maui.Controls;

namespace MusicAlbum_Explorer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Create window with AppShell as root
            var window = new Window(new AppShell());

            // Force a phone-like window size (width x height)
            // Width = 450, Height = 850 (landscape/portrait as requested)
            // On platforms that don't support setting size (mobile), these values will be ignored.
            window.Width = 450;
            window.Height = 850;

            return window;
        }
    }
}