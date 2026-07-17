using AppManager.Data.LocalDbSafeDeleter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AppManager.Data
{
    public class AppManagerDbDeleteDbService : ITransientDependency
    {
        private readonly IConfiguration _config;
        public ILogger<AppManagerDbDeleteDbService> Logger { get; set; }

        public AppManagerDbDeleteDbService(IConfiguration config)
        {
            _config=config;
        }

        public Task  DoAsync(string[] args)
        {
            Logger.LogInformation("=== LocalDB 安全删除工具 ===");

            // 你的 LocalDB 连接字符串
            string connectionString = _config.GetConnectionString("Default")!;

            string dbName = ExtractDatabaseName(connectionString);
            Logger.LogInformation($"目标数据库: {dbName}");
            Logger.LogInformation("====================================");

            // 确认操作
            Logger.LogInformation("⚠️  确认删除该数据库？(输入 YES 确认): ");

            // 执行安全删除
            bool success = LocalDbDeleter.DeleteLocalDatabaseSafely(connectionString);

            if (success)
            {
                Logger.LogInformation("\n✅ 操作成功完成。");
            }
            else
            {
                Logger.LogInformation("\n❌ 操作失败，请查看上方错误信息。");
            }

            Logger.LogInformation("\n按任意键退出...");
            Console.ReadKey();
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
