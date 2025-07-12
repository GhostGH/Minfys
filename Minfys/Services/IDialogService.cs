using Minfys.ViewModels.Dialogs;

namespace Minfys.Services;

/// <summary>
/// Provides a contract for showing modal dialog windows with view models.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Opens a dialog window using its View Model type and return both the dialog result and the custom result.
    /// </summary>
    /// <typeparam name="TViewModel">The type of dialog's View Model which must implement IRequestCloseViewModel.</typeparam>
    /// <typeparam name="TResult">Additional data that may be returned from a dialog when it closes.</typeparam>
    /// <returns>A tuple containing: DialogResult (bool?) indicating how the dialog was closed, and 
    /// Result (TResult) containing the data returned from the dialog.</returns>
    (bool? DialogResult, TResult? Result) ShowDialog<TViewModel, TResult>()
        where TViewModel : class, IRequestCloseViewModel<TResult>;
}