using Dapper;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using System.Data;
using System.Threading.Tasks;

namespace InbentorySystem.Services
{
    public class ZaikoService : IZaikoService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ZaikoService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task UpdateQuantityAsync(string shohinCode, int quantityDiff)
        {
            const string sql = @"
                UPDATE T_ZAIKO
                SET currentquantity = currentquantity + @QuantityDiff,
                    kousinnichiji = CURRENT_TIMESTAMP
                WHERE shohincode = @ShohinCode;";

            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(sql, new { shohinCode = shohinCode, QuantityDiff = quantityDiff });
        }

        public async Task<int> GetCurrentQuantityAsync(string shohinCode)
        {
            const string sql = @"
                SELECT currentquantity
                FROM T_ZAIKO
                WHERE shohincode = @ShohinCode;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>(sql, new {ShohinCode = shohinCode});
        }

        public async Task<bool> IsStockSufficientAsync(string shohinCode, int requiredQuantity)
        {
            var current = await GetCurrentQuantityAsync(shohinCode);
            return current >= requiredQuantity;
        }
    }

}