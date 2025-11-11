using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using InbentorySystem.Infrastructure.Interfaces;

namespace InbentorySystem.Data
{
    /// <summary>
    /// データベースとリポジトリ層の間に入り、Dapperをラップして使用するクラス
    /// 要は、受け取ったSQL文を実行するクラス
    /// </summary>
    public class SqlExecutor : ISqlExecutor
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public SqlExecutor(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// SQL文を実行し、影響を受けた行数を返す
        /// </summary>
        /// <param name="sql">string:実行するsql文</param>
        /// <param name="param">object?:sqlにバインドするパラメータparam等</param>
        /// <param name="transaction">トランザクション制御用オブジェクト</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sql, param, transaction);
        }

        /// <summary>
        /// SQL文を実行し、複数行の結果を IEnumerable<T> として返す。
        /// </summary>
        /// <param name="connection">接続文字列</param>
        /// <param name="sql">string:実行するsql文</param>
        /// <param name="param">object?:sqlにバインドするパラメータ</param>
        /// <param name="transaction">トランザクション制御用オブジェクト</param>
        /// <returns>複数行の結果</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return await connection.QueryAsync<T>(sql, param, transaction);
        }

        /// <summary>
        /// SQL文を実行し、最初の1行を T 型で返す。該当なしの場合は null。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection">接続文字列</param>
        /// <param name="sql">string:実行するsql文</param>
        /// <param name="param">object?:sqlにバインドするパラメータ</param>
        /// <param name="transaction">トランザクション制御用オブジェクト</param>
        /// <returns>最初の1行</returns>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        /// <summary>
        /// SQL文を実行し、1件のみの結果を T 型で返す。該当なしの場合は null。
        /// </summary>
        /// <param name="connection">接続文字列</param>
        /// <param name="sql">string:実行するsql文</param>
        /// <param name="param">object?:sqlにバインドするパラメータ</param>
        /// <param name="transaction">トランザクション制御用オブジェクト</param>
        /// <returns>1件のみの結果</returns>
        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
        }
    }
}
