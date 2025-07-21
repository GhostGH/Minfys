using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Minfys.ViewModels.Dialogs;

namespace Minfys.Services;

/// <summary>
/// Manages opening and closing windows.
/// </summary>
public class DialogService : IDialogService
{
    private readonly IServiceProvider _services;

    public DialogService(IServiceProvider services)
    {
        _services = services;
    }

    public (bool? DialogResult, TResult? Result) ShowDialog<TViewModel, TResult>()
        where TViewModel : class, IRequestCloseViewModel<TResult>
    {
        var viewModel = _services.GetRequiredService<TViewModel>();

        // Try finding a Window with the right DataContext or name match
        var windowType = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(t =>
                typeof(Window).IsAssignableFrom(t) &&
                t.Name.Replace("Dialog", "DialogViewModel") == typeof(TViewModel).Name);

        if (windowType == null)
            throw new InvalidOperationException($"Window not found for {typeof(TViewModel).Name}");

        var window = (Window)_services.GetRequiredService(windowType);
        window.Owner = Application.Current.MainWindow;
        window.DataContext = viewModel;

        if (Application.Current.MainWindow != null)
        {
            window.Owner = Application.Current.MainWindow;
            window.Owner.Opacity = 0.7;
        }

        TResult? result = default;

        viewModel.RequestClose += (s, e) =>
        {
            result = e.Result;
            window.DialogResult = e.DialogResult;
            window.Close();

            if (window.Owner != null)
            {
                window.Owner.Opacity = 1;
            }
        };

        var dialogResult = window.ShowDialog();

        return (dialogResult, result);
    }

    public bool? ShowDialog<TViewModel>() where TViewModel : class, IRequestCloseViewModel<object>
    {
        var viewModel = _services.GetRequiredService<TViewModel>();

        // Try finding a Window with the right DataContext or name match
        var windowType = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(t =>
                typeof(Window).IsAssignableFrom(t) &&
                t.Name.Replace("Dialog", "DialogViewModel") == typeof(TViewModel).Name);

        if (windowType == null)
            throw new InvalidOperationException($"Window not found for {typeof(TViewModel).Name}");

        var window = (Window)_services.GetRequiredService(windowType);
        window.Owner = Application.Current.MainWindow;
        window.DataContext = viewModel;

        if (Application.Current.MainWindow != null)
        {
            window.Owner = Application.Current.MainWindow;
            window.Owner.Opacity = 0.7;
        }


        viewModel.RequestClose += (s, e) =>
        {
            window.DialogResult = e.DialogResult;
            window.Close();

            if (window.Owner != null)
            {
                window.Owner.Opacity = 1;
            }
        };

        return window.ShowDialog();
    }
}