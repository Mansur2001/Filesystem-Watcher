namespace FilesystemWatcher.Tests
{
    public class AppTests
    {
        [Fact]
        public void App_CanBeCreated()
        {
            var app = new App();
            Assert.NotNull(app);
        }

        [Fact]
        public void App_InitializesFramework()
        {
            var app = new App();
            app.Initialize();

            // Call OnFrameworkInitializationCompleted safely
            app.OnFrameworkInitializationCompleted();

            Assert.True(app.Styles.Count > 0); // Rough check that app initialized
        }
    }
}