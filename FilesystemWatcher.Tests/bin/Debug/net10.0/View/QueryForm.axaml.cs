using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using FilesystemWatcher.ViewModel;

namespace FilesystemWatcher.View
{
    public partial class QueryForm : Window
    {
        public QueryForm() : this(new QueryCriteriaViewModel()) { }

        public QueryForm(QueryCriteriaViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // Auto-refresh all rows on open
            this.Opened += async (_, __) =>
            {
                viewModel.SelectedExtension = null;
                await viewModel.RunExtensionQueryCommand.Execute();
            };
        }

        private void InitializeComponent()
            => AvaloniaXamlLoader.Load(this);

        private void CloseQueryButton_Click(object? sender, RoutedEventArgs e)
            => Close();
    }
}