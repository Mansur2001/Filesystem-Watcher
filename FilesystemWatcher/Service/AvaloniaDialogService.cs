using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;

namespace FilesystemWatcher.Service
{
    /// <summary>
    /// Provides dialog implementations for alerts, confirmations, prompts, and file save dialogs
    /// using Avalonia UI windows.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public class AvaloniaDialogService : IDialogService
    {
        /// <summary>
        /// Shows a simple alert dialog with a title and message.
        /// </summary>
        /// <param name="title">The title of the alert window.</param>
        /// <param name="message">The message to display in the alert.</param>
        /// <returns>A task that completes when the alert is closed.</returns>
        public Task Alert(string title, string message)
        {
            var dlg = new Window
            {
                Title = title,
                Content = new TextBlock { Text = message, Margin = new Thickness(10) },
                Width = 400,
                Height = 200
            };

            var main = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            return dlg.ShowDialog(main);
        }

        /// <summary>
        /// Shows a confirmation dialog with "OK" and "Cancel" buttons.
        /// </summary>
        /// <param name="title">The title of the confirmation window.</param>
        /// <param name="message">The message to display asking for confirmation.</param>
        /// <returns>
        /// A task that completes with <c>true</c> if the user clicks OK, or <c>false</c> if the user clicks Cancel.
        /// </returns>
        public async Task<bool> Confirm(string title, string message)
        {
            var dlg = new Window
            {
                Title = title,
                Width = 400,
                Height = 200
            };

            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10)
            };
            panel.Children.Add(new TextBlock { Text = message });

            var btnPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0)
            };

            var ok = new Button { Content = "OK", Width = 80, Margin = new Thickness(5) };
            var cancel = new Button { Content = "Cancel", Width = 80, Margin = new Thickness(5) };

            btnPanel.Children.Add(ok);
            btnPanel.Children.Add(cancel);
            panel.Children.Add(btnPanel);
            dlg.Content = panel;

            var main = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            var tcs = new TaskCompletionSource<bool>();

            ok.Click += (_, __) => { tcs.TrySetResult(true); dlg.Close(); };
            cancel.Click += (_, __) => { tcs.TrySetResult(false); dlg.Close(); };

            dlg.ShowDialog(main);
            return await tcs.Task;
        }

        /// <summary>
        /// Opens a save-file dialog to choose a path for exporting data.
        /// </summary>
        /// <param name="title">The title of the save-file dialog window.</param>
        /// <param name="initialFileName">The suggested initial file name.</param>
        /// <returns>
        /// A task that completes with the chosen file path, or <c>null</c> if the dialog was canceled.
        /// </returns>
        public Task<string?> SaveFileDialog(string title, string initialFileName)
        {
            var dlg = new SaveFileDialog
            {
                Title = title,
                InitialFileName = initialFileName
            };

            var main = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            return dlg.ShowAsync(main!);
        }

        /// <summary>
        /// Shows a prompt dialog with a text input field.
        /// </summary>
        /// <param name="title">The title of the prompt window.</param>
        /// <param name="message">The message or question to display above the input field.</param>
        /// <returns>
        /// A task that completes with the entered text, or <c>null</c> if the dialog was closed without input.
        /// </returns>
        public Task<string?> Prompt(string title, string message)
        {
            var dlg = new Window
            {
                Title = title,
                Width = 400,
                Height = 200
            };

            var panel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10)
            };
            panel.Children.Add(new TextBlock { Text = message });

            var input = new TextBox { Margin = new Thickness(0, 10, 0, 0) };
            panel.Children.Add(input);

            var btn = new Button
            {
                Content = "OK",
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 10, 0, 0)
            };
            panel.Children.Add(btn);

            dlg.Content = panel;

            var main = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            var tcs = new TaskCompletionSource<string?>();

            btn.Click += (_, __) => { tcs.TrySetResult(input.Text); dlg.Close(); };

            dlg.ShowDialog(main);
            return tcs.Task;
        }
    }
}
