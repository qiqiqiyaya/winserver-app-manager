using System;
using Microsoft.Web.Administration;
using Volo.Abp.DependencyInjection;

namespace AppManager.IisSite;

/// <summary>
/// ServerManager 内存池，管理所有 IIS 实例的 ServerManager 对象。
/// Singleton 生命周期，应用停止时自动释放所有 ServerManager。
/// </summary>
public interface IServerManagerPool : ISingletonDependency, IDisposable
{
    /// <summary>
    /// 根据 configPath 获取或创建 ServerManager 实例。
    /// </summary>
    /// <param name="configPath">applicationHost.config 路径。为空时使用系统默认。</param>
    ServerManager GetOrCreate(string? configPath);
}
