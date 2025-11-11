using Dapper;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Services.Interfaces;
using System.Data;
using System.Threading.Tasks;

namespace InbentorySystem.Services
{
    public class ZaikoService : IZaikoService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        /// <summary>
        /// DB接続生成用のファクトリを注入。
        /// </summary>
        public ZaikoService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        /// <summary>
        /// 指定された商品コードの在庫数量を加算・減算する。
        /// </summary>
        /// <param name="shohinCode">対象の商品コード</param>
        /// <param name="quantityDiff">加減算する数量</param>
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

        /// <summary>
        /// 指定された商品コードの現在の在庫数量を取得する
        /// </summary>
        /// <param name="shohinCode">対象の商品コード</param>
        /// <returns>現在の在庫数</returns>
        public async Task<int> GetCurrentQuantityAsync(string shohinCode)
        {
            const string sql = @"
                SELECT currentquantity
                FROM T_ZAIKO
                WHERE shohincode = @ShohinCode;";

            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int>(sql, new { ShohinCode = shohinCode });
        }

        /// <summary>
        /// 指定された商品コードの在庫が、必要数量以上あるかを判定する。
        /// </summary>
        /// <param name="shohinCode">対象の商品コード</param>
        /// <param name="requiredQuantity">必要な数量</param>
        /// <returns>足りてたらtrue/足りていなかったらfalse</returns>
        public async Task<bool> IsStockSufficientAsync(string shohinCode, int requiredQuantity)
        {
            var current = await GetCurrentQuantityAsync(shohinCode);
            return current >= requiredQuantity;
        }
    }
}