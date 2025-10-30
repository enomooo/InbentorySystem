using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace InbentorySystem.Data
{
    public interface ISqlExecutor
    {
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null);
    }
}