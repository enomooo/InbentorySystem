using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace InbentorySystem.Data
{
    /// <summary>
    /// DapperによるSQL実行を抽象化するインターフェース
    /// </summary>
    public interface ISqlExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null);
        Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
    }
}