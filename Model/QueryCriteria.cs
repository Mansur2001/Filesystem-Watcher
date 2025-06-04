namespace FilesystemWatcher.Model
{
    public class QueryCriteria
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Extension { get; set; }
        public string EventType { get; set; }
        public string DirectoryPath { get; set; }
    }
}