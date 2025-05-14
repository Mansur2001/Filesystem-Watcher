// FileWatcherViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FileWatcherApp.ViewModels
{
    public class FileWatcherViewModel
    {
        private readonly DatabaseManager myDatabaseManager;
        private readonly CSVExporter myCsvExporter;
        private readonly EmailService myEmailService;
        private FileSystemWatcher myFileSystemWatcher;
        private readonly Dispatcher myDispatcher;

        public ObservableCollection<FileEventViewModel> myFileEvents { get; } = new();

        public FileWatcherViewModel(DatabaseManager theDatabaseManager, CSVExporter theCsvExporter, EmailService theEmailService, Dispatcher theDispatcher)
        {
            myDatabaseManager = theDatabaseManager;
            myCsvExporter = theCsvExporter;
            myEmailService = theEmailService;
            myDispatcher = theDispatcher;
        }

        public void StartWatching(string theExtension, string thePath)
        {
            if (!Directory.Exists(thePath))
            {
                MessageBox.Show("Selected directory does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            myFileSystemWatcher = new FileSystemWatcher(thePath)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                Filter = "*" + theExtension
            };

            myFileSystemWatcher.Created += OnFileChanged;
            myFileSystemWatcher.Changed += OnFileChanged;
            myFileSystemWatcher.Deleted += OnFileChanged;
            myFileSystemWatcher.Renamed += OnFileRenamed;
        }

        public void StopWatching()
        {
            if (myFileSystemWatcher != null)
            {
                myFileSystemWatcher.EnableRaisingEvents = false;
                myFileSystemWatcher.Dispose();
                myFileSystemWatcher = null;
            }
        }

        public void SaveEventsToDB()
        {
            try
            {
                foreach (var theEvent in myFileEvents.Select(theViewModel => theViewModel.myModel))
                {
                    myDatabaseManager.InsertEvent(theEvent);
                }
                MessageBox.Show("Events saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception theException)
            {
                MessageBox.Show($"Failed to save events: {theException.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ObservableCollection<FileEventViewModel> QueryEvents(QueryCriteriaViewModel theCriteriaViewModel)
        {
            var myCriteria = theCriteriaViewModel.myModel;
            var myEvents = myDatabaseManager.QueryEvents(myCriteria);
            return new ObservableCollection<FileEventViewModel>(
                myEvents.Select(theEvent => new FileEventViewModel(theEvent)));
        }

        public void ExportToCSV(string theFilePath, QueryCriteriaViewModel theCriteriaViewModel)
        {
            try
            {
                var myEvents = myDatabaseManager.QueryEvents(theCriteriaViewModel.myModel);
                myCsvExporter.Export(myEvents, theFilePath);
                MessageBox.Show("Export successful.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception theException)
            {
                MessageBox.Show($"Export failed: {theException.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void SendCSVEmail(string theRecipientEmail, string theFilePath)
        {
            try
            {
                myEmailService.SendEmail(theRecipientEmail, theFilePath);
                MessageBox.Show("Email sent successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception theException)
            {
                MessageBox.Show($"Failed to send email: {theException.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnFileChanged(object theSender, FileSystemEventArgs theEventArgs)
        {
            var myFileEvent = new FileEvent
            {
                FileName = Path.GetFileName(theEventArgs.FullPath),
                Extension = Path.GetExtension(theEventArgs.FullPath),
                Path = theEventArgs.FullPath,
                EventType = theEventArgs.ChangeType.ToString(),
                Timestamp = DateTime.UtcNow
            };

            myDispatcher.Invoke(() => myFileEvents.Add(new FileEventViewModel(myFileEvent)));
        }

        private void OnFileRenamed(object theSender, RenamedEventArgs theEventArgs)
        {
            var myFileEvent = new FileEvent
            {
                FileName = Path.GetFileName(theEventArgs.FullPath),
                Extension = Path.GetExtension(theEventArgs.FullPath),
                Path = theEventArgs.FullPath,
                EventType = "Renamed",
                Timestamp = DateTime.UtcNow
            };

            myDispatcher.Invoke(() => myFileEvents.Add(new FileEventViewModel(myFileEvent)));
        }
    }
}
