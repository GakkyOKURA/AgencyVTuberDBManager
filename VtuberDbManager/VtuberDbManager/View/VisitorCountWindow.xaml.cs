using System.Windows;
using VtuberDbManager.ViewModel;

namespace VtuberDbManager.View;

/// <summary>
/// VisitorCountWindow.xaml の相互作用ロジック
/// </summary>
public partial class VisitorCountWindow : Window
{
    private readonly VisitorCountWindowViewModel _vm;
    public VisitorCountWindow(VisitorCountWindowViewModel viewModel)
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
