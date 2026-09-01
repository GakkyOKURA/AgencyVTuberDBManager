using DynamicData;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using VtuberDbManager.Model;
using VtuberDbManager.Service;
using VtuberDbManager.View;
using VtuberDbManager.ViewModel.Base;

namespace VtuberDbManager.ViewModel;

public class PlatformWindowViewModel : ViewModelBase, IDisposable
{
    private bool _disposed;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IVtuberDbService _vtuberDbService;
    internal CompositeDisposable Disposables { get; } = new();
    public ReactiveCollection<PlatformReactive> Platforms { get; }
    public ReactivePropertySlim<string> NewName { get; set; }
    public AsyncReactiveCommand AddCommand { get; }
    public AsyncReactiveCommand UpdateCommand { get; }
    public AsyncReactiveCommand DeleteCommand { get; }
    public ReactiveCommandSlim ContentRenderedCommand { get; }
    public ReactivePropertySlim<PlatformReactive?> SelectedPlatform { get; set; }
    private PlatformReactive? _lastSelectedPlatform;

    public PlatformWindowViewModel(IMessageBoxService messageBoxService, IVtuberDbService vtuberDbService)
    {
        _messageBoxService = messageBoxService;
        _vtuberDbService = vtuberDbService;

        Platforms = new ReactiveCollection<PlatformReactive>().AddTo(Disposables);
        NewName = new ReactivePropertySlim<string>().AddTo(Disposables);
        SelectedPlatform = new ReactivePropertySlim<PlatformReactive?>().AddTo(Disposables);

        AddCommand = NewName
            .Select(v => !string.IsNullOrWhiteSpace(v))
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);

        var selectedObserve = SelectedPlatform
            .Select(v => v is not null);
        UpdateCommand = selectedObserve
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);
        DeleteCommand = selectedObserve
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);

        ContentRenderedCommand = new ReactiveCommandSlim().AddTo(Disposables);

        SubscribeProperty();
        SubscribeCommand();

    }

    private void SubscribeProperty()
    {
        SelectedPlatform.Subscribe(v =>
        {
            _lastSelectedPlatform?.SetOrigiinal();
            _lastSelectedPlatform = v;
        }).AddTo(Disposables);
    }

    private void SubscribeCommand()
    {
        AddCommand.Subscribe(async () =>
        {
            NewName.Value = NewName.Value.Trim();

            var newData = new PlatformTable()
            {
                Name = NewName.Value
            };

            var addResult = await _vtuberDbService.AddPlatformAsync(newData);
            if (addResult.IsSuccess)
            {
                newData.Id = addResult.Id;
                Platforms.Add(new PlatformReactive(newData).AddTo(Disposables));
            }

            _messageBoxService.Show(addResult.Message, nameof(PlatformWindow));

            NewName.Value = "";
        }).AddTo(Disposables);

        UpdateCommand.Subscribe(async () =>
        {
            SelectedPlatform.Value!.Name.Value = SelectedPlatform.Value.Name.Value.Trim();

            var updateData = new PlatformTable()
            {
                Id = SelectedPlatform.Value.Id,
                Name = SelectedPlatform.Value.Name.Value
            };

            var updateResult = await _vtuberDbService.UpdatePlatformAsync(updateData);
            if (updateResult.IsSuccess)
            {
                // 元のデータをアップデート
                SelectedPlatform.Value.UpdateOrignal();
            }

            _messageBoxService.Show(updateResult.Message, nameof(GroupsWindow));
        }).AddTo(Disposables);

        DeleteCommand.Subscribe(async () =>
        {
            var deleteResult = await _vtuberDbService.DeletePlatformAsync(SelectedPlatform.Value!.Id);
            if (deleteResult.IsSuccess)
            {
                Platforms.Remove(SelectedPlatform.Value);
            }

            _messageBoxService.Show(deleteResult.Message, nameof(GroupsWindow));
        }).AddTo(Disposables);

        ContentRenderedCommand.Subscribe(async () =>
        {
            await GetDataFromDB();
        }).AddTo(Disposables);
    }

    private async Task GetDataFromDB()
    {
        var platforms = await _vtuberDbService.GetAllPlatformsAsync();
        Platforms.AddRange(platforms.Items
            .Select(v => new PlatformReactive(v).AddTo(Disposables)));
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
