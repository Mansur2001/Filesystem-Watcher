using Xunit;
using System;
using FilesystemWatcher.Model;

namespace FilesystemWatcher.Tests
{
    public class FileEventTests
    {
        [Fact]
        public void FileEvent_CanBeCreatedWithProperties()
        {
            var timestamp = new DateTime(2025, 6, 1, 12, 0, 0);

            var fileEvent = new FileEvent
            {
                FileName = "example.txt",
                Extension = ".txt",
                FilePath = "/path/example.txt",
                EventType = "Created",
                Timestamp = timestamp
            };

            Assert.Equal("example.txt", fileEvent.FileName);
            Assert.Equal(".txt", fileEvent.Extension);
            Assert.Equal("/path/example.txt", fileEvent.FilePath);
            Assert.Equal("Created", fileEvent.EventType);
            Assert.Equal(timestamp, fileEvent.Timestamp);
        }
    }
}