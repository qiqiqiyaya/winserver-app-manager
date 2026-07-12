using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Web.Administration;
using Volo.Abp.DependencyInjection;

namespace AppManager.IisSite;

/// <summary>
/// ServerManager 内存池实现。
/// 线程安全，ConcurrentDictionary 管理，应用停止时释放所有实例。
/// </summary>
public class ServerManagerPool : IServerManagerPool, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, ServerManager> _managers =
        new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;

    public ServerManager GetOrCreate(string? configPath)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var key = NormalizeKey(configPath);
        return _managers.GetOrAdd(key, _ => CreateServerManager(configPath));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var kvp in _managers)
        {
            try { kvp.Value.Dispose(); } catch { }
        }
        _managers.Clear();
    }

    private static ServerManager CreateServerManager(string? configPath)
    {
        return string.IsNullOrWhiteSpace(configPath)
            ? new ServerManager()
            : new ServerManager(configPath);
    }

    private static string NormalizeKey(string? configPath)
    {
        if (string.IsNullOrWhiteSpace(configPath))
            return "__DEFAULT__";
        return Path.GetFullPath(configPath).ToLowerInvariant();
    }
}
