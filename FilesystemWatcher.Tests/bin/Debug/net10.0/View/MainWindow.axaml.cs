using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FilesystemWatcher.Service;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ONLY here do we set DataContext now that XAML no longer tries:
            var watcherService  = new FileWatcherService();
            var csvExporter     = new CSVExporter();
            var databaseManager = new DatabaseManager();
            var emailService    = new EmailService();

            DataContext = new FileWatcherViewModel(
                watcherService,
                csvExporter,
                databaseManager,
                emailService);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}