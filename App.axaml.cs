using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FilesystemWatcher.View;
using FilesystemWatcher.ViewModel;
using FilesystemWatcher.Service;
using FilesystemWatcher.Factory;

namespace FilesystemWatcher
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var dbManager = new DatabaseManager("Data Source=events.db;");
                var csvExporter = new CSVExporter();
                var emailService = new EmailService();
                var factory = new ViewModelFactory();

                var mainVM = new FileWatcherViewModel(dbManager, csvExporter, emailService);

                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainVM
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}