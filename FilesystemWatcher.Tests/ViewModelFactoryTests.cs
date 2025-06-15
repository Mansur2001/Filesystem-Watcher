using Xunit;
using FilesystemWatcher.Factory;
using FilesystemWatcher.Service;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.Tests
{
    public class ViewModelFactoryTests
    {
        private readonly ViewModelFactory _factory;

        public ViewModelFactoryTests()
        {
            var watcherService  = new FileWatcherService();
            var csvExporter     = new CSVExporter();
            var databaseManager = new DatabaseManager(":memory:");
            var emailService    = new EmailService();
            _factory = new ViewModelFactory(watcherService, csvExporter, databaseManager, emailService);
        }

        [Fact]
        public void CreateFileWatcherViewModel_ReturnsNotNull()
        {
            var vm = _factory.CreateFileWatcherViewModel();
            Assert.NotNull(vm);
        }

        [Fact]
        public void CreateQueryCriteriaViewModel_ReturnsNotNull()
        {
            var vm = _factory.CreateQueryCriteriaViewModel();
            Assert.NotNull(vm);
        }
    }
}