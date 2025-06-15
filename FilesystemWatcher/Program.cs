using Avalonia;
using Avalonia.ReactiveUI;

namespace FilesystemWatcher
{
    /// <summary>
    /// Entry point for the Avalonia application, responsible for configuring
    /// and launching the desktop lifetime.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    internal class Program
    {
        /// <summary>
        /// Application main method. Builds and starts the Avalonia app
        /// with a classic desktop-style lifetime.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
            => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// Configures the Avalonia <see cref="AppBuilder"/>, enabling platform detection,
        /// trace logging, and ReactiveUI support.
        /// </summary>
        /// <returns>A configured <see cref="AppBuilder"/> instance.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}