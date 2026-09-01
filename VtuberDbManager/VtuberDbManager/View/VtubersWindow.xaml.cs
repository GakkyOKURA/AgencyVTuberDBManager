using System.Windows;
using VtuberDbManager.ViewModel;

namespace VtuberDbManager.View;

/// <summary>
/// VtubersWindow.xaml の相互作用ロジック
/// </summary>
public partial class VtubersWindow : Window
{
    private readonly VtubersWindowViewModel _vm;
    public VtubersWindow(VtubersWindowViewModel viewModel)
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
