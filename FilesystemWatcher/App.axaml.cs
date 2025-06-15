using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FilesystemWatcher.Service;
using FilesystemWatcher.View;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher
{
    /// <summary>
    /// Application entry point: loads XAML, configures services, and
    /// initializes the main window with its ViewModel.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public partial class App : Application
    {
        /// <summary>
        /// Loads the XAML definitions for the application.
        /// </summary>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Called when Avalonia has finished initializing the framework.
        /// Sets up shared services, creates the root ViewModel, and
        /// assigns the MainWindow to the desktop lifetime.
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 1) Instantiate shared services
                var dialogService  = new AvaloniaDialogService();
                var watcherService = new FileWatcherService();
                var dbManager      = new DatabaseManager();
                var csvExporter    = new CSVExporter();
                var emailService   = new EmailService();

                // 2) Create the main ViewModel, injecting those services
                var viewModel = new FileWatcherViewModel(
                    watcherService,
                    csvExporter,
                    dbManager,
                    emailService,
                    dialogService);

                // 3) Construct the main window and assign its DataContext
                var window = new MainWindow
                {
                    DataContext = viewModel
                };

                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
