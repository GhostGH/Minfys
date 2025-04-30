using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace Minfys.Services;

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

        var window = (Window)_services.GetRequiredService(windowType); // uses XAML-defined type
        window.Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
        window.DataContext = viewModel;

        Window ownerWindow = window.Owner!;
        ownerWindow.Opacity = 0.7;

        TResult? result = default;

        viewModel.RequestClose += (s, e) =>
        {
            result = e.Result;
            window.DialogResult = e.DialogResult;
            window.Close();
            ownerWindow.Opacity = 1;
        };

        var dialogResult = window.ShowDialog();

        return (dialogResult, result);
    }
}