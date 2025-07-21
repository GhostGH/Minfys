namespace Minfys.ViewModels.Dialogs;

/// <summary>
/// Contract that allows dialog windows to be closed.
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IRequestCloseViewModel<TResult>
{
    event EventHandler<RequestCloseDialogEventArgs<TResult>> RequestClose;
}