namespace FilesystemWatcher.Model
{
    /// <summary>
    /// Represents a single file system event, including its name, path, type, and timestamp.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class FileEvent
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// Gets or sets the file extension (including the leading dot).
        /// </summary>
        public string Extension { get; set; } = "";

        /// <summary>
        /// Gets or sets the full path to the file.
        /// </summary>
        public string FilePath { get; set; } = "";

        /// <summary>
        /// Gets or sets the type of event (e.g., "Created", "Changed", "Deleted", "Renamed", "Manual").
        /// </summary>
        public string EventType { get; set; } = "";

        /// <summary>
        /// Gets or sets the timestamp when the event occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}