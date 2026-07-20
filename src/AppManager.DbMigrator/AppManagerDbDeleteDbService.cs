using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Volo.Abp.DependencyInjection;

namespace AppManager.DbMigrator
{
    public class AppManagerDbDeleteDbService : ITransientDependency
    {
        private readonly IConfiguration _config;
        public ILogger<AppManagerDbDeleteDbService> Logger { get; set; }
        private readonly LocalDbDeleter _localDbDeleter;

        public AppManagerDbDeleteDbService(IConfiguration config, LocalDbDeleter localDbDeleter)
        {
            _config = config;
            _localDbDeleter = localDbDeleter;
        }

        public void Do()
        {
            Logger.LogInformation("=== LocalDB 安全删除工具 ===");

            // 你的 LocalDB 连接字符串
            string connectionString = _config.GetConnectionString("Default")!;

            string dbName = ExtractDatabaseName(connectionString);
            Logger.LogInformation($"目标数据库: {dbName}");
            Logger.LogInformation("====================================");

            // 执行安全删除
            bool success = _localDbDeleter.DeleteLocalDatabaseSafely(connectionString);

            if (success)
            {
                Logger.LogInformation("操作成功完成。");
            }
            else
            {
                Logger.LogInformation("操作失败，请查看上方错误信息。");
            }
        }

        /// <summary>
        /// 从连接字符串提取数据库名（辅助方法）
        /// </summary>
        private static string ExtractDatabaseName(string connectionString)
        {
            var match = Regex.Match(connectionString, @"(Initial\s*Catalog|Database)\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value.Trim() : null;
        }
    }
}
