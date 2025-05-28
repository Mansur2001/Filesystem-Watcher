// MainWindow.xaml.cs
// Mansur Yassin & Tairan Zhang
using System;
using System.Windows;

namespace FileWatcherApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.
    /// Connects UI buttons to FileWatcherViewModel actions.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly FileWatcherViewModel myViewModel;

        /// <summary>
        /// Initializes the MainWindow with its ViewModel.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            myViewModel = new FileWatcherViewModel(...); // Inject dependencies here
            DataContext = myViewModel;
        }

        /// <summary>
        /// Starts file monitoring.
        /// </summary>
        public void bindStartMonitoring()
        {
            myViewModel.StartWatching(...); // Pass extension and path
        }

        /// <summary>
        /// Stops file monitoring.
        /// </summary>
        public void bindStopMonitoring()
        {
            myViewModel.StopWatching();
        }

        /// <summary>
        /// Saves monitored events to the database.
        /// </summary>
        public void bindSaveToDatabase()
        {
            myViewModel.SaveEventsToDB();
        }

        /// <summary>
        /// Queries the database for events.
        /// </summary>
        public void bindQueryDatabase()
        {
            var result = myViewModel.QueryEvents(...); // Pass QueryCriteriaViewModel
            // Display result in UI
        }

        /// <summary>
        /// Exports queried events to CSV.
        /// </summary>
        public void bindExportToCSV()
        {
            myViewModel.ExportToCSV(..., ...); // filePath, criteria
        }

        /// <summary>
        /// Sends the CSV file via email.
        /// </summary>
        public void bindSendEmail()
        {
            myViewModel.SendCSVEmail(..., ...); // recipient, filepath
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        public void bindExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
