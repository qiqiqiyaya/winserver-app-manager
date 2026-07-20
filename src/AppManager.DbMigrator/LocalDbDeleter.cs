using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using Volo.Abp.DependencyInjection;

namespace AppManager.DbMigrator
{
    public class LocalDbDeleter : ITransientDependency
    {
        public ILogger<LocalDbDeleter> Logger { get; set; }

        /// <summary>
        /// 安全删除 LocalDB 数据库（先设置单用户模式，强制断开连接）
        /// </summary>
        /// <param name="connectionString">完整的连接字符串（必须指向 LocalDB）</param>
        /// <returns>是否成功</returns>
        public bool DeleteLocalDatabaseSafely(string connectionString)
        {
            // 1. 验证是否为 LocalDB
            if (!IsLocalDbConnection(connectionString))
            {
                Logger.LogInformation("安全限制：仅允许删除 LocalDB 数据库，当前连接不是 LocalDB。");
                return false;
            }

            // 2. 提取数据库名称
            string dbName = ExtractDatabaseNameFromConnStr(connectionString);
            if (string.IsNullOrEmpty(dbName))
            {
                Logger.LogInformation("无法从连接字符串中提取数据库名称。");
                return false;
            }

            Logger.LogInformation($"检测到 LocalDB 连接，目标数据库: {dbName}");

            // 3. 构建连接到 master 的连接字符串（替换数据库名）
            string masterConnectionString = ReplaceDatabaseName(connectionString, "master");

            // 4. 执行删除（带重试机制）
            return ForceDropWithRetry(masterConnectionString, dbName, maxRetries: 3);
        }

        /// <summary>
        /// 判断连接字符串是否指向 LocalDB
        /// </summary>
        private bool IsLocalDbConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                return false;

            // 方式1：正则匹配 Data Source
            var match = Regex.Match(connectionString, @"Data\s*Source\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string dataSource = match.Groups[1].Value.Trim();
                if (dataSource.Contains("(localdb)", StringComparison.OrdinalIgnoreCase) ||
                    dataSource.Contains("LocalDB", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            // 方式2：直接包含关键字
            return connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 从连接字符串提取数据库名
        /// </summary>
        private string ExtractDatabaseNameFromConnStr(string connectionString)
        {
            var match = Regex.Match(connectionString, @"(Initial\s*Catalog|Database)\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[2].Value.Trim() : null;
        }

        /// <summary>
        /// 替换连接字符串中的数据库名
        /// </summary>
        private string ReplaceDatabaseName(string connectionString, string newDbName)
        {
            string pattern = @"(Initial\s*Catalog|Database)\s*=\s*[^;]+";
            string replacement = $"$1={newDbName}";

            if (Regex.IsMatch(connectionString, pattern, RegexOptions.IgnoreCase))
            {
                return Regex.Replace(connectionString, pattern, replacement, RegexOptions.IgnoreCase);
            }
            else
            {
                // 如果没有指定数据库，追加
                return connectionString.TrimEnd(';') + $";Database={newDbName};";
            }
        }

        /// <summary>
        /// 带重试的强制删除
        /// </summary>
        private bool ForceDropWithRetry(string masterConnectionString, string dbName, int maxRetries)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    Logger.LogInformation($"第 {attempt} 次尝试删除...");
                    if (ForceDropDatabaseWithSingleUser(masterConnectionString, dbName))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogInformation($"第 {attempt} 次尝试失败: {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    Logger.LogInformation($"等待 1 秒后重试...");
                    Thread.Sleep(1000);
                }
            }

            Logger.LogInformation($"所有 {maxRetries} 次重试均失败。");
            return false;
        }

        /// <summary>
        /// 核心方法：设置单用户模式 → 删除数据库
        /// </summary>
        private bool ForceDropDatabaseWithSingleUser(string masterConnectionString, string dbName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    Logger.LogInformation("已连接到 master 数据库。");

                    // 1. 检查数据库是否存在
                    if (!DatabaseExists(conn, dbName))
                    {
                        Logger.LogInformation($"数据库 '{dbName}' 不存在，无需删除。");
                        return true;
                    }

                    // 2. 设置单用户模式，强制断开所有连接
                    Logger.LogInformation($"正在设置单用户模式，强制断开所有连接...");
                    string setSingleUserSql = $@"
                        ALTER DATABASE [{dbName}] 
                        SET SINGLE_USER 
                        WITH ROLLBACK IMMEDIATE;
                    ";

                    using (SqlCommand cmd = new SqlCommand(setSingleUserSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.LogInformation("已成功设置为单用户模式，所有连接已断开。");
                    }

                    // 3. 删除数据库
                    Logger.LogInformation($"正在删除数据库 '{dbName}'...");
                    string dropSql = $"DROP DATABASE [{dbName}];";

                    using (SqlCommand cmd = new SqlCommand(dropSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.LogInformation($"数据库 '{dbName}' 已成功删除。");
                        return true;
                    }
                }
            }
            catch (SqlException ex) when (ex.Number == 3702)
            {
                Logger.LogInformation($"数据库 '{dbName}' 仍在使用中，无法删除。");
                Logger.LogInformation($"错误详情: {ex.Message}");
                return false;
            }
            catch (SqlException ex) when (ex.Number == 5061 || ex.Number == 5070)
            {
                Logger.LogInformation($"无法设置单用户模式，尝试备用方案...");
                // 备用方案：先设为多用户再设为单用户
                return TryAlternativeDrop(masterConnectionString, dbName);
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"删除失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 检查数据库是否存在
        /// </summary>
        private bool DatabaseExists(SqlConnection conn, string dbName)
        {
            string sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName";
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@dbName", dbName);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// 备用删除方案：重置状态后再尝试
        /// </summary>
        private bool TryAlternativeDrop(string masterConnectionString, string dbName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    Logger.LogInformation("备用方案：重置数据库状态...");

                    // 先设为多用户
                    string setMultiUserSql = $"ALTER DATABASE [{dbName}] SET MULTI_USER;";
                    using (SqlCommand cmd = new SqlCommand(setMultiUserSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    Thread.Sleep(500);

                    // 再设为单用户
                    string setSingleUserSql = $@"
                        ALTER DATABASE [{dbName}] 
                        SET SINGLE_USER 
                        WITH ROLLBACK IMMEDIATE;
                    ";
                    using (SqlCommand cmd = new SqlCommand(setSingleUserSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 删除
                    string dropSql = $"DROP DATABASE [{dbName}];";
                    using (SqlCommand cmd = new SqlCommand(dropSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                        Logger.LogInformation($"通过备用方案成功删除数据库 '{dbName}'。");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"备用方案失败: {ex.Message}");
                return false;
            }
        }
    }
}
