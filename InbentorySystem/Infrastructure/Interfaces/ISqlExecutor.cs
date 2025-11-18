using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace InbentorySystem.Infrastructure.Interfaces
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

        /// <summary>
        /// トランザクション内で複数のSQL文を実行します。
        /// </summary>
        /// <param name="sql">実行するSQL文（複数の文を含む）</param>
        /// <param name="param">SQLに渡すパラメータ</param>
        /// <returns>影響を与えた行数</returns>
        Task<int> ExecuteInTransactionAsync(string sql, object param);
    }
}