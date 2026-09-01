using System.Windows;
using VtuberDbManager.Service;
using VtuberDbManager.ViewModel;

namespace VtuberDbManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(IWindowService windowService, MainiWindowViewModel viewModel)
    {
        windowService.SetMapping(viewModel, this);

        InitializeComponent();
        DataContext = viewModel;
    }
}