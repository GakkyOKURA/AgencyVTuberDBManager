using System.Windows;
using VtuberDbManager.ViewModel.Base;

namespace VtuberDbManager.Service;

public interface IWindowService
{
    void SetMapping(ViewModelBase viewModel, Window window);
    void Show<TWindow>(ViewModelBase viewModel) where TWindow : Window;
    bool? ShowDialog<TWindow>(ViewModelBase viewModel) where TWindow : Window;
    void Close(ViewModelBase viewModel);
}
