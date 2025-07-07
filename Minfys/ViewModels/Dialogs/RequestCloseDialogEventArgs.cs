namespace Minfys.ViewModels.Windows;

public class RequestCloseDialogEventArgs<TResult> : EventArgs
{
    public bool? DialogResult { get; }
    public TResult? Result { get; }

    public RequestCloseDialogEventArgs(bool? dialogResult, TResult? result = default)
    {
        DialogResult = dialogResult;
        Result = result;
    }
}