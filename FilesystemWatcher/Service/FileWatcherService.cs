using FilesystemWatcher.Model;

namespace FilesystemWatcher.Service
{
    /// <summary>
    /// Wraps <see cref="FileSystemWatcher"/> to monitor a directory for file system events
    /// (create, change, delete, rename) and raises them as <see cref="FileEvent"/> instances.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class FileWatcherService
    {
        /// <summary>
        /// The underlying <see cref="FileSystemWatcher"/> instance.
        /// </summary>
        private FileSystemWatcher? _watcher;

        /// <summary>
        /// Occurs when a file system event is detected. Subscribers receive a <see cref="FileEvent"/>.
        /// </summary>
        public event EventHandler<FileEvent>? OnFileEvent;

        /// <summary>
        /// Starts monitoring the specified directory for files matching the given extension.
        /// </summary>
        /// <param name="directory">The directory path to watch.</param>
        /// <param name="extension">
        /// The file extension filter (including the dot), e.g. ".txt"; all files matching "*" + extension are monitored.
        /// </param>
        public void StartWatching(string directory, string extension)
        {
            // Tear down any existing watcher.
            _watcher?.Dispose();

            _watcher = new FileSystemWatcher(directory, "*" + extension)
            {
                IncludeSubdirectories = false,
                // Watch for file name, last write time, and file size changes.
                NotifyFilter = NotifyFilters.FileName
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Size
            };

            _watcher.Created += (s, e) => Raise(e.Name, e.FullPath, "Created");
            _watcher.Changed += (s, e) => Raise(e.Name, e.FullPath, "Changed");
            _watcher.Deleted += (s, e) => Raise(e.Name, e.FullPath, "Deleted");
            _watcher.Renamed += (s, e) => Raise(e.Name, e.FullPath, "Renamed");

            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stops monitoring and releases the underlying watcher.
        /// </summary>
        public void StopWatching()
        {
            _watcher?.Dispose();
            _watcher = null;
        }

        /// <summary>
        /// Constructs a <see cref="FileEvent"/> and raises the <see cref="OnFileEvent"/> event.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <param name="path">The full file path.</param>
        /// <param name="type">The type of event ("Created", "Changed", "Deleted", "Renamed").</param>
        private void Raise(string name, string path, string type)
        {
            OnFileEvent?.Invoke(this, new FileEvent
            {
                FileName  = name,
                FilePath  = path,
                Extension = Path.GetExtension(name),
                EventType = type,
                Timestamp = DateTime.Now
            });
        }
    }
}
