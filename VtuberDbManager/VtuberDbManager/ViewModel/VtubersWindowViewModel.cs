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

public class VtubersWindowViewModel : ViewModelBase, IDisposable
{
    private bool _disposed;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IVtuberDbService _vtuberDbService;
    private readonly ITwitchIdService _twitchIdService;
    internal CompositeDisposable Disposables { get; } = new();
    public ReactiveCollection<VtuberReactive> Vtubers { get; }
    public ReactivePropertySlim<string> SearchName { get; set; }
    public ReactivePropertySlim<string?> SearchGroup { get; set; }
    public ReactivePropertySlim<string?> SearchPlatform { get; set; }
    public ReactivePropertySlim<string> NewName { get; set; }
    public ReactivePropertySlim<string> NewGroupName { get; set; }
    public ReactivePropertySlim<string> NewPlatform { get; set; }
    public ReactivePropertySlim<string> NewChannelId { get; set; }
    public ReactivePropertySlim<string> TwitchLoginId { get; set; }
    public ReactivePropertySlim<string> TwitchId { get; set; }
    public List<string> Groups { get; } = new();
    public List<string> Platforms { get; } = new();
    public AsyncReactiveCommand AddCommand { get; }
    public AsyncReactiveCommand UpdateCommand { get; }
    public AsyncReactiveCommand DeleteCommand { get; }
    public AsyncReactiveCommand SearchCommand { get; }
    public AsyncReactiveCommand SearchAllCommand { get; }
    public AsyncReactiveCommand SearchTwitchIdCommand { get; }
    public ReactiveCommandSlim ClearGroupCommand { get; }
    public ReactiveCommandSlim ClearPlatformCommand { get; }
    public ReactiveCommandSlim ClearNameCommand { get; }
    public ReactiveCommandSlim ClearTwitchLoginIdCommand { get; }
    public ReactiveCommandSlim ContentRenderedCommand { get; }

    /// <summary>
    /// 現在選択している Vtuber データ
    /// </summary>
    public ReactivePropertySlim<VtuberReactive?> SelectedVtuber { get; set; }

    /// <summary>
    /// 直近で選択していた Vtuber データ。データを選択して情報を変更後、
    /// 「選択行を更新」ボタンを押さないと更新できないようにしている。
    /// 情報変更後、当該ボタンを押さずに他の Vtuber データを選択した際は、
    /// 変更前の情報に戻す必要がある。その時に、このフィールドを使用する。
    /// </summary>
    private VtuberReactive? _lastSelectedVtuber;

    public VtubersWindowViewModel(
        IMessageBoxService messageBoxService,
        IVtuberDbService vtuberDbService,
        ITwitchIdService twitchIdService)
    {
        _messageBoxService = messageBoxService;
        _vtuberDbService = vtuberDbService;
        _twitchIdService = twitchIdService;

        Vtubers = new ReactiveCollection<VtuberReactive>().AddTo(Disposables);
        SearchName = new ReactivePropertySlim<string>("").AddTo(Disposables);
        SearchGroup = new ReactivePropertySlim<string?>().AddTo(Disposables);
        SearchPlatform = new ReactivePropertySlim<string?>().AddTo(Disposables);
        NewName = new ReactivePropertySlim<string>().AddTo(Disposables);
        NewGroupName = new ReactivePropertySlim<string>().AddTo(Disposables);
        NewPlatform = new ReactivePropertySlim<string>().AddTo(Disposables);
        NewChannelId = new ReactivePropertySlim<string>().AddTo(Disposables);
        SelectedVtuber = new ReactivePropertySlim<VtuberReactive?>().AddTo(Disposables);
        TwitchLoginId = new ReactivePropertySlim<string>().AddTo(Disposables);
        TwitchId = new ReactivePropertySlim<string>().AddTo(Disposables);
        ContentRenderedCommand = new ReactiveCommandSlim().AddTo(Disposables);

        // 各プロパティに情報が入力されているときだけ有効に
        AddCommand = new[]
        {
            NewName.Select(v => !string.IsNullOrWhiteSpace(v)),
            NewGroupName.Select(v => !string.IsNullOrWhiteSpace(v)),
            NewPlatform.Select(v => !string.IsNullOrWhiteSpace(v)),
            NewChannelId.Select(v => !string.IsNullOrWhiteSpace(v)),
        }
        .CombineLatestValuesAreAllTrue()
        .ToAsyncReactiveCommand()
        .AddTo(Disposables);

        var selectedObserve = SelectedVtuber
            .Select(v => v is not null);
        UpdateCommand = selectedObserve
            .ToAsyncReactiveCommand()
            .AddTo (Disposables);
        DeleteCommand = selectedObserve
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);

        // いずれかに情報が入力されているときだけ有効に
        var canSearch = Observable
            .CombineLatest(
            SearchName,
            SearchGroup,
            SearchPlatform,
            (name, group, platform) => 
            !string.IsNullOrWhiteSpace(name) 
            || !string.IsNullOrEmpty(group) 
            || !string.IsNullOrEmpty(platform)
            );
        SearchCommand = canSearch
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);

        SearchAllCommand = new AsyncReactiveCommand().AddTo(Disposables);

        SearchTwitchIdCommand = TwitchLoginId
            .Select(v => !string.IsNullOrWhiteSpace(v))
            .ToAsyncReactiveCommand()
            .AddTo(Disposables);

        ClearGroupCommand = SearchGroup
            .Select(v => v is not null)
            .ToReactiveCommandSlim()
            .AddTo(Disposables);

        ClearPlatformCommand = SearchPlatform
            .Select(v => v is not null)
            .ToReactiveCommandSlim()
            .AddTo(Disposables);

        ClearNameCommand = SearchName
            .Select(v => !string.IsNullOrEmpty(v))
            .ToReactiveCommandSlim()
            .AddTo(Disposables);

        ClearTwitchLoginIdCommand = TwitchLoginId
            .Select(v => !string.IsNullOrEmpty(v))
            .ToReactiveCommandSlim()
            .AddTo(Disposables);

        SubscribeProperty();
        SubscribeCommand();
    }

    private void SubscribeProperty()
    {
        SelectedVtuber.Subscribe(v =>
        {
            _lastSelectedVtuber?.SetOrigiinal();
            _lastSelectedVtuber = v;
        }).AddTo(Disposables);
    }

    private void SubscribeCommand()
    {
        AddCommand.Subscribe(async () =>
        {
            NewName.Value = NewName.Value.Trim();
            NewChannelId.Value = NewChannelId.Value.Trim();

            var newData = new VtuberDTO()
            {
                Name = NewName.Value,
                GroupName = NewGroupName.Value,
                PlatformName = NewPlatform.Value,
                ChannelId = NewChannelId.Value,
            };

            var addResult = await _vtuberDbService.AddVtuberAsync(newData);
            if (addResult.IsSuccess)
            {
                newData.Id = addResult.Id;
                Vtubers.Add(new VtuberReactive(newData).AddTo(Disposables));
            }

            _messageBoxService.Show(addResult.Message, nameof(VtubersWindow));

            NewName.Value = "";
            NewChannelId.Value = "";
        }).AddTo(Disposables);

        UpdateCommand.Subscribe(async () =>
        {
            SelectedVtuber.Value!.Name.Value = SelectedVtuber.Value.Name.Value.Trim();

            var updateData = new VtuberDTO()
            {
                Id = SelectedVtuber.Value.Id,
                Name = SelectedVtuber.Value.Name.Value,
                GroupName = SelectedVtuber.Value.Group.Value,
                PlatformName = SelectedVtuber.Value.Platform.Value,
                ChannelId = SelectedVtuber.Value.ChannelId.Value
            };

            var updateResult = await _vtuberDbService.UpdateVtuberAsync(updateData);
            if (updateResult.IsSuccess)
            {
                SelectedVtuber.Value.UpdateOrignal();
            }

            _messageBoxService.Show(updateResult.Message, nameof(VtubersWindow));
        }).AddTo(Disposables);

        DeleteCommand.Subscribe(async () =>
        {
            var deleteResult = await _vtuberDbService.DeleteVtuberAsync(SelectedVtuber.Value!.Id);
            if (deleteResult.IsSuccess)
            {
                Vtubers.Remove(SelectedVtuber.Value);
            }

            _messageBoxService.Show(deleteResult.Message, nameof(VtubersWindow));
        }).AddTo(Disposables);

        SearchCommand.Subscribe(async () =>
        {
            ClearVtubers();

            SearchName.Value = SearchName.Value.Trim();
            var searchName = SearchName.Value;
            var searchGroup = SearchGroup.Value ?? "";
            var searchPlatform = SearchPlatform.Value ?? "";
            var searchResult = await _vtuberDbService.GetVtubersByFilterAsync(searchName, searchGroup, searchPlatform);
            Vtubers.AddRange(
                searchResult.Items
                .Select(v => new VtuberReactive(v).AddTo(Disposables)));
        }).AddTo(Disposables);

        SearchAllCommand.Subscribe(async () =>
        {
            ClearVtubers();

            var vtubers = await _vtuberDbService.GetAllVtubersAsync();
            Vtubers.AddRange(
                vtubers.Items
                .Select(v => new VtuberReactive(v).AddTo(Disposables)));
        }).AddTo(Disposables);

        SearchTwitchIdCommand.Subscribe(async () =>
        {
            var twitchId = await _twitchIdService.GetUserIdFromLoginIdAsync(TwitchLoginId.Value);
            TwitchId.Value = twitchId ?? "";
        }).AddTo(Disposables);

        ContentRenderedCommand.Subscribe(async () =>
        {
            await GetDataFromDB();
        }).AddTo(Disposables);

        ClearGroupCommand.Subscribe(() => SearchGroup.Value = null)
            .AddTo(Disposables);

        ClearPlatformCommand.Subscribe(() => SearchPlatform.Value = null)
            .AddTo(Disposables);

        ClearNameCommand.Subscribe(() => SearchName.Value = "")
            .AddTo(Disposables);

        ClearTwitchLoginIdCommand.Subscribe(() => TwitchLoginId.Value = "")
            .AddTo(Disposables);
    }

    private void ClearVtubers()
    {
        foreach (var v in Vtubers)
        {
            v.Dispose();
        }
        Vtubers.Clear();
    }

    private async Task GetDataFromDB()
    {
        var vtubers = await _vtuberDbService.GetAllVtubersAsync();
        Vtubers.AddRange(
            vtubers.Items
            .Select(v => new VtuberReactive(v).AddTo(Disposables)));

        var groups = await _vtuberDbService.GetAllGroupsAsync();
        Groups.AddRange(groups.Items
            .Select(v => v.Name));

        var platforms = await _vtuberDbService.GetAllPlatformsAsync();
        Platforms.AddRange(platforms.Items
            .Select(v => v.Name));
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
