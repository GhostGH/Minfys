namespace Minfys.Services;

/// <summary>
/// Provide a contract for showing message and error pop-ups to the user.
/// </summary>
public interface IMessageService
{
    /// <summary>
    /// Shows an error message pop-up to the user.
    /// </summary>
    /// <param name="message">The text of the message.</param>
    void ShowError(string message);

    /// <summary>
    /// Shows a message pop-up to the user.
    /// </summary>
    /// <param name="message">The text of the message.</param>
    void ShowMessage(string message);
}