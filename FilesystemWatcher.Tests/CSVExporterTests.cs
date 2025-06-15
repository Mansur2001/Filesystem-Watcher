using FilesystemWatcher.Model;
using FilesystemWatcher.Service;

namespace FilesystemWatcher.Tests
{
    public class CSVExporterTests
    {
        [Fact]
        public void Export_CreatesValidCSVFile()
        {
            var events = new List<FileEvent>
            {
                new FileEvent
                {
                    FileName = "file1.txt",
                    Extension = ".txt",
                    FilePath = "/tmp/file1.txt",
                    EventType = "Created",
                    Timestamp = new DateTime(2023, 1, 1, 10, 0, 0)
                },
                new FileEvent
                {
                    FileName = "file2.log",
                    Extension = ".log",
                    FilePath = "/tmp/file2.log",
                    EventType = "Deleted",
                    Timestamp = new DateTime(2023, 1, 1, 11, 0, 0)
                }
            };

            var path = Path.GetTempFileName();
            var exporter = new CSVExporter();

            exporter.Export(events, path);

            Assert.True(File.Exists(path));
            var contents = File.ReadAllText(path);
            Assert.Contains("file1.txt", contents);
            Assert.Contains("file2.log", contents);
            Assert.Contains("Created", contents);
            Assert.Contains("Deleted", contents);
        }
    }
}