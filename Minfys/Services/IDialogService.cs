namespace Minfys.Services;

public interface IDialogService
{
    /// <summary>
    /// Opens a dialog window using its View Model type and returns both the dialog result and the custom result.
    /// </summary>
    /// <typeparam name="TViewModel">The type of dialog's View Model which must implement IRequestCloseViewModel</typeparam>
    /// <typeparam name="TResult">The type of value that's being returned when dialog closes</typeparam>
    /// <returns>A tuple containing: DialogResult (bool?) indicating how the dialog was closed, and 
    /// Result (TResult) containing the data returned from the dialog</returns>
    (bool? DialogResult, TResult? Result) ShowDialog<TViewModel, TResult>()
        where TViewModel : class, IRequestCloseViewModel<TResult>;
}