namespace Minfys.ViewModels.Dialogs;

/// <summary>
/// Contract that allows dialog windows to be closed.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IRequestCloseViewModel<TResult>
{
    /// <summary>
    /// Is used for closing windows by DialogService.
    /// </summary>
    event EventHandler<RequestCloseDialogEventArgs<TResult>> RequestClose;
}