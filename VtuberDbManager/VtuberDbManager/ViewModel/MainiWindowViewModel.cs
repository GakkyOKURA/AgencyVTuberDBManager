using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using VtuberDbManager.Service;
using VtuberDbManager.View;
using VtuberDbManager.ViewModel.Base;

namespace VtuberDbManager.ViewModel;

public class MainiWindowViewModel : ViewModelBase
{
    private readonly IWindowService _windowService;
    internal CompositeDisposable Disposables { get; } = new();
    public ReactiveCommandSlim ShowVtubersDataCommand { get; }
    public ReactiveCommandSlim ShowGroupsDataCommand { get; }
    public ReactiveCommandSlim ShowPlatformsDataCommand { get; }
    public ReactiveCommandSlim ShowVisitorCountCommand { get; }
    public ReactiveCommandSlim CloseCommand { get; }

    public MainiWindowViewModel(IWindowService windowService)
    {
        _windowService = windowService;

        ShowVtubersDataCommand = new ReactiveCommandSlim().AddTo(Disposables);
        ShowGroupsDataCommand = new ReactiveCommandSlim().AddTo(Disposables);
        ShowPlatformsDataCommand = new ReactiveCommandSlim().AddTo(Disposables);
        ShowVisitorCountCommand = new ReactiveCommandSlim().AddTo(Disposables);
        CloseCommand = new ReactiveCommandSlim().AddTo(Disposables);

        SubscribeCommands();
    }

    private void SubscribeCommands()
    {
        ShowVtubersDataCommand.Subscribe(() =>
        {
            _windowService.ShowDialog<VtubersWindow>(this);
        }).AddTo(Disposables);

        ShowGroupsDataCommand.Subscribe(() =>
        {
            _windowService.ShowDialog<GroupsWindow>(this);
        }).AddTo(Disposables);

        ShowPlatformsDataCommand.Subscribe(() =>
        {
            _windowService.ShowDialog<PlatformWindow>(this);
        }).AddTo(Disposables);

        ShowVisitorCountCommand.Subscribe(() =>
        {
            _windowService.ShowDialog<VisitorCountWindow>(this);
        }).AddTo(Disposables);

        CloseCommand.Subscribe(() =>
        {
            _windowService.Close(this);
        }).AddTo(Disposables);
    }
}
