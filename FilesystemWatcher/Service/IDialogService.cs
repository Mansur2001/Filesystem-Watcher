namespace FilesystemWatcher.Service
{
    /// <summary>
    /// Provides a unified interface for displaying dialogs within the application,
    /// including alerts, confirmations, file save dialogs, and text prompts.
    /// </summary>
    /// <author>Mansur Yassin</author>
    /// <author>Tairan Zhang</author>
    public interface IDialogService
    {
        /// <summary>
        /// Shows an informational alert dialog with a title and message.
        /// </summary>
        /// <param name="title">The title of the alert window.</param>
        /// <param name="message">The message to display to the user.</param>
        /// <returns>A task that completes when the user closes the alert.</returns>
        Task Alert(string title, string message);

        /// <summary>
        /// Shows a confirmation dialog with "OK" and "Cancel" buttons.
        /// </summary>
        /// <param name="title">The title of the confirmation dialog.</param>
        /// <param name="message">The message asking the user to confirm.</param>
        /// <returns>
        /// A task that completes with <c>true</c> if the user confirms (OK),
        /// or <c>false</c> if the user cancels.
        /// </returns>
        Task<bool> Confirm(string title, string message);

        /// <summary>
        /// Opens a file save dialog for the user to choose where to save a file.
        /// </summary>
        /// <param name="title">The title of the save file dialog.</param>
        /// <param name="initialFileName">The suggested initial file name.</param>
        /// <returns>
        /// A task that completes with the selected file path, or <c>null</c>
        /// if the dialog is canceled.
        /// </returns>
        Task<string?> SaveFileDialog(string title, string initialFileName);

        /// <summary>
        /// Shows a prompt dialog with a text input field for the user.
        /// </summary>
        /// <param name="title">The title of the prompt dialog.</param>
        /// <param name="message">The message or question to display above the input.</param>
        /// <returns>
        /// A task that completes with the entered string, or <c>null</c>
        /// if the dialog is closed without input.
        /// </returns>
        Task<string?> Prompt(string title, string message);
    }
}
