using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VtuberDbManager.ViewModel.Base;

namespace VtuberDbManager.Service;

internal class WindowService : IWindowService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Window の探索でよく使われる
    /// 
    /// Application.Current.Windows
    ///     .OfType<TWindow>()
    ///     .FirstOrDefault();
    ///     
    /// では TWindow が複数開いているときに
    /// 意図したものが取れるかの信頼性が無い。
    /// なので Dictionary で管理する
    /// </summary>
    private readonly Dictionary<ViewModelBase, Window> _viewModelToWindow = new();

    public WindowService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Window 側で呼び出す
    /// </summary>
    public void SetMapping(ViewModelBase viewModel, Window window)
    {
        _viewModelToWindow.Add(viewModel, window);
        // この時点で remove を登録しておく
        window.Closed += (_, _) => _viewModelToWindow.Remove(viewModel);
    }

    public void Show<TWindow>(ViewModelBase viewModel) where TWindow : Window
    {
        _viewModelToWindow.TryGetValue(viewModel, out var ownerWindow);

        var window = _serviceProvider.GetRequiredService<TWindow>();
        window.Owner = ownerWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Show();
    }

    public bool? ShowDialog<TWindow>(ViewModelBase viewModel) where TWindow: Window
    {
        _viewModelToWindow.TryGetValue(viewModel, out var ownerWindow);

        var window = _serviceProvider.GetRequiredService<TWindow>();
        window.Owner = ownerWindow;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        return window.ShowDialog();
    }

    public void Close(ViewModelBase viewModel)
    {
        _viewModelToWindow.TryGetValue(viewModel, out var window);
        window?.Close();
    }
}
