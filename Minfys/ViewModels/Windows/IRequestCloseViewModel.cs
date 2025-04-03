using Minfys.ViewModels.Windows;

namespace Minfys.Services;

public interface IRequestCloseViewModel<TResult>
{
    event EventHandler<RequestCloseDialogEventArgs<TResult>> RequestClose;
}