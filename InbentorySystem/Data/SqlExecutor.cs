using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

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

        public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sql, param, transaction);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction=null)
        {
            return await connection.QueryAsync<T>(sql, param, transaction);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction);
        }
    }
}
