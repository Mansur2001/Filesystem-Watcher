namespace FilesystemWatcher.Model
{
    public class FileEvent
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}