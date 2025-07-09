namespace Minfys.ViewModels.Dialogs;

public interface IRequestCloseViewModel<TResult>
{
    event EventHandler<RequestCloseDialogEventArgs<TResult>> RequestClose;
}