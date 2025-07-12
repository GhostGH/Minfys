using System.Windows;

namespace Minfys.Services;

/// <summary>
/// Manages error and message pop-ups.
/// </summary>
public class MessageService : IMessageService
{
    public void ShowError(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}