using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace InbentorySystem.Data
{
    public class SqlExecutor : ISqlExecutor
    {
        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction=null)
        {
            return await connection.QueryAsync<T>(sql, param, transaction);
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }
    }
}
