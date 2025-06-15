using FilesystemWatcher.Model;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.Tests
{
    public class FileEventViewModelTests
    {
        [Fact]
        public void Constructor_SetsModelAndPropertiesCorrectly()
        {
            var model = new FileEvent
            {
                FileName = "log.txt",
                Extension = ".txt",
                FilePath = "/logs/log.txt",
                EventType = "Created",
                Timestamp = new DateTime(2023, 1, 1, 10, 0, 0)
            };

            var vm = new FileEventViewModel(model)
            {
                RowNumber = 1
            };

            Assert.Equal("log.txt", vm.FileName);
            Assert.Equal(".txt", vm.Extension);
            Assert.Equal("/logs/log.txt", vm.Model.FilePath);
            Assert.Equal("Created", vm.EventType);
            Assert.Equal("2023-01-01 10:00:00", vm.FormattedTimestamp);
            Assert.Equal(1, vm.RowNumber);
        }
    }
}