using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using VtuberDbManager.Service;
using VtuberDbManager.ViewModel.Base;

namespace VtuberDbManager.ViewModel;

public class VisitorCountWindowViewModel : ViewModelBase, IDisposable
{
    private bool _disposed;
    private readonly IVtuberDbService _vtuberDbService;

    internal CompositeDisposable Disposables { get; } = new();
    public ReactivePropertySlim<string> VisitorCount { get; set; }
    public ReactiveCommandSlim ContentRenderedCommand { get; }

    public VisitorCountWindowViewModel(IVtuberDbService vtuberDbService)
    {
        _vtuberDbService = vtuberDbService;
        VisitorCount = new ReactivePropertySlim<string>().AddTo(Disposables);
        ContentRenderedCommand = new ReactiveCommandSlim().AddTo(Disposables);

        SubscribeCommand();
    }

    private void SubscribeCommand()
    {
        ContentRenderedCommand.Subscribe(async () =>
        {
            await GetDataFromDB();
        }).AddTo(Disposables);
    }

    private async Task GetDataFromDB()
    {
        var visitorCount = await _vtuberDbService.GetCountAsync();
        VisitorCount.Value = visitorCount.ToString();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Disposables.Dispose();
        }

        _disposed = true;
    }
}