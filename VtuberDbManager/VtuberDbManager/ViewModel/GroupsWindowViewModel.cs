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

public class GroupsWindowViewModel : ViewModelBase, IDisposable
{
    private bool _disposed;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IVtuberDbService _vtuberDbService;
    internal CompositeDisposable Disposables { get; } = new();
    public ReactiveCollection<GroupReactive> Groups { get; }
    public ReactivePropertySlim<string> NewName { get; set; }
    public AsyncReactiveCommand AddCommand { get; }
    public AsyncReactiveCommand UpdateCommand { get; }
    public AsyncReactiveCommand DeleteCommand { get; }
    public ReactiveCommandSlim ContentRenderedCommand { get; }
    public ReactivePropertySlim<GroupReactive?> SelectedGroup { get; set; }
    private GroupReactive? _lastSelectedGroup;
    public GroupsWindowViewModel(IMessageBoxService messageBoxService, IVtuberDbService vtuberDbService)
    {
        _messageBoxService = messageBoxService;
        _vtuberDbService = vtuberDbService;

        Groups = new ReactiveCollection<GroupReactive>().AddTo(Disposables);
        NewName = new ReactivePropertySlim<string>().AddTo(Disposables);
        SelectedGroup = new ReactivePropertySlim<GroupReactive?>().AddTo(Disposables);

        AddCommand = NewName
            .Select(v => !string.IsNullOrWhiteSpace(v))
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);

        var selectedObserve = SelectedGroup
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
        SelectedGroup.Subscribe(v =>
        {
            _lastSelectedGroup?.SetOrigiinal();
            _lastSelectedGroup = v;
        }).AddTo(Disposables);
    }

    private void SubscribeCommand()
    {
        AddCommand.Subscribe(async () =>
        {
            NewName.Value = NewName.Value.Trim();

            var newData = new GroupTable()
            {
                Name = NewName.Value
            };

            var addResult = await _vtuberDbService.AddGroupAsync(newData);
            if(addResult.IsSuccess)
            {
                newData.Id = addResult.Id;
                Groups.Add(new GroupReactive(newData));
            }

            _messageBoxService.Show(addResult.Message, nameof(GroupsWindow));

            NewName.Value = "";
        }).AddTo(Disposables);

        UpdateCommand.Subscribe(async () =>
        {
            SelectedGroup.Value!.Name.Value = SelectedGroup.Value.Name.Value.Trim();

            var updateData = new GroupTable()
            {
                Id = SelectedGroup.Value.Id,
                Name = SelectedGroup.Value.Name.Value
            };

            var updateResult = await _vtuberDbService.UpdateGroupAsync(updateData);
            if (updateResult.IsSuccess)
            {
                // 元のデータをアップデート
                SelectedGroup.Value.UpdateOrignal();
            }

            _messageBoxService.Show(updateResult.Message, nameof(GroupsWindow));
        }).AddTo(Disposables);

        DeleteCommand.Subscribe(async () =>
        {
            var deleteResult = await _vtuberDbService.DeleteGroupAsync(SelectedGroup.Value!.Id);
            if (deleteResult.IsSuccess)
            {
                Groups.Remove(SelectedGroup.Value);
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
        var groups = await _vtuberDbService.GetAllGroupsAsync();
        Groups.AddRange(groups.Items
            .Select(v => new GroupReactive(v).AddTo(Disposables)));
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
