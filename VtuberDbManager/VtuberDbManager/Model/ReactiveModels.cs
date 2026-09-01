using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Reactive.Disposables;

namespace VtuberDbManager.Model;

public class VtuberReactive : IDisposable
{
    private bool _disposed;
    private CompositeDisposable _disposables = new();
    internal int Id { get; }
    public ReactivePropertySlim<string> Name { get; set; }
    private string _originalName;
    public ReactivePropertySlim<string> Group { get; set; }
    private string _originalGroup;
    public ReactivePropertySlim<string> Platform { get; set; }
    private string _originalPlatform;
    public ReactivePropertySlim<string> ChannelId { get; set; }
    private string _originalChannelId;

    public VtuberReactive(VtuberDTO dto)
    {
        Id = dto.Id;

        Name = new ReactivePropertySlim<string>(dto.Name).AddTo(_disposables);
        _originalName = dto.Name;

        Group = new ReactivePropertySlim<string>(dto.GroupName).AddTo(_disposables);
        _originalGroup = dto.GroupName;

        Platform = new ReactivePropertySlim<string>(dto.PlatformName).AddTo(_disposables);
        _originalPlatform = dto.PlatformName;

        ChannelId = new ReactivePropertySlim<string>(dto.ChannelId).AddTo(_disposables);
        _originalChannelId = dto.ChannelId;
    }

    internal void UpdateOrignal()
    {
        _originalName = Name.Value;
        _originalGroup = Group.Value;
        _originalPlatform = Platform.Value;
        _originalChannelId= ChannelId.Value;
    }

    internal void SetOrigiinal()
    {
        Name.Value = _originalName;
        Group.Value = _originalGroup;
        Platform.Value = _originalPlatform;
        ChannelId.Value = _originalChannelId;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if(_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposables.Dispose();
        }

        _disposed = true;
    }
}

public class GroupReactive : IDisposable
{
    private bool _disposed;
    public int Id { get; set; }
    public ReactivePropertySlim<string> Name { get; set; }
    private string _originalName;

    public GroupReactive(GroupTable dto)
    {
        Id = dto.Id;
        Name = new(dto.Name);
        _originalName = dto.Name;
    }

    internal void UpdateOrignal()
    {
        _originalName = Name.Value;
    }

    internal void SetOrigiinal()
    {
        Name.Value = _originalName;
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
            Name.Dispose();
        }

        _disposed = true;
    }
}

public class PlatformReactive : IDisposable
{
    private bool _disposed;
    public int Id { get; set; }
    public ReactivePropertySlim<string> Name { get; set; }
    private string _originalName;

    public PlatformReactive(PlatformTable dto)
    {
        Id = dto.Id;
        Name = new(dto.Name);
        _originalName = dto.Name;
    }

    internal void UpdateOrignal()
    {
        _originalName = Name.Value;
    }

    internal void SetOrigiinal()
    {
        Name.Value = _originalName;
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
            Name.Dispose();
        }

        _disposed = true;
    }
}