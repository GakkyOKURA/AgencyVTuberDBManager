using System.Windows;
using VtuberDbManager.ViewModel;

namespace VtuberDbManager.View;

/// <summary>
/// GroupsWindow.xaml の相互作用ロジック
/// </summary>
public partial class GroupsWindow : Window
{
    private readonly GroupsWindowViewModel _vm;
    public GroupsWindow(GroupsWindowViewModel viewModel)
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
