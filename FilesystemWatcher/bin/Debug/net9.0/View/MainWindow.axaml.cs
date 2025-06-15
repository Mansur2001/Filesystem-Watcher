using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FilesystemWatcher.Service;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.View
{
    /// <summary>
    /// The main application window. Initializes UI components and sets up
    /// the <see cref="FileWatcherViewModel"/> as its DataContext.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Loads the XAML, constructs the required services and ViewModel,
        /// and assigns it to <see cref="DataContext"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            var vm = new FileWatcherViewModel(
                new FileWatcherService(),
                new CSVExporter(),
                new DatabaseManager("events.db"),
                new EmailService());

            DataContext = vm;
        }

        /// <summary>
        /// Loads the XAML markup for this window.
        /// </summary>
        private void InitializeComponent() 
            => AvaloniaXamlLoader.Load(this);
    }
}