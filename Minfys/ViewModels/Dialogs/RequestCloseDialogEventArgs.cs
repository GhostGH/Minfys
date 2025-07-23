namespace Minfys.ViewModels.Dialogs;

/// <summary>
/// Serves for passing arguments for IRequestCloseViewModel.
/// </summary>
/// <typeparam name="TResult">Type of additional optional data to pass to the caller from the dialog.</typeparam>
public class RequestCloseDialogEventArgs<TResult> : EventArgs
{
    public bool? DialogResult { get; }
    public TResult? Result { get; }

    /// <summary>
    /// Creates arguments to pass to the caller when the dialog closes.
    /// </summary>
    /// <param name="dialogResult">Whether dialog was operation was accepted or canceled.</param>
    /// <param name="result">Additional optional data to pass to the caller from the dialog.</param>
    public RequestCloseDialogEventArgs(bool? dialogResult, TResult? result = default)
    {
        DialogResult = dialogResult;
        Result = result;
    }
}