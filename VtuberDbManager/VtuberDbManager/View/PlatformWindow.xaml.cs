using System.Windows;
using VtuberDbManager.ViewModel;

namespace VtuberDbManager.View;

/// <summary>
/// PlatformWindow.xaml の相互作用ロジック
/// </summary>
public partial class PlatformWindow : Window
{
    private readonly PlatformWindowViewModel _vm;
    public PlatformWindow(PlatformWindowViewModel viewModel)
    {
        InitializeComponent();
        _vm = viewModel;
        DataContext = _vm;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        _vm.Dispose();
    }
}
