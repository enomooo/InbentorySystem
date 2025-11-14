using InbentorySystem.Data.Models;
using System.Data;
using Dapper;
using System.Linq;
using InbentorySystem.Infrastructure.Interfaces;

namespace InbentorySystem.Infrastructure.Repository
{
    public class ShiireRepository : IShiireRepository
    {
        // 依存性の注入 (IDbConnectionFactoryとISqlExecutorは既存のものを使用)
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ISqlExecutor _executor;

        public ShiireRepository(IDbConnectionFactory connectionFactory, ISqlExecutor executor)
        {
            _connectionFactory = connectionFactory;
            _executor = executor;
        }

        public async Task<int> RegisterAsync(ShiireModel shiire)
        {
            shiire.Tourokunichiji = DateTime.Now;
            const string sql = @"
                INSERT INTO T_SHIIRE (shiiresakinengappi, shohincode, shiiresakicode, quantity, shiirene, tourokunichiji) 
                VALUES (@ShiireNengappi, @ShohinCode, @ShiiresakiCode, @Quantity, @Shiirene, @TourokuNichiji);

                INSERT INTO T_ZAIKO(shohincode, currentquantity, tourokunichiji, kousinnichiji) 
                VALUES (@ShohinCode, @Quantity, @TourokuNichiji, @TourokuNichiji) 

                ON CONFLICT (shohincode) 
                DO UPDATE SET currentquantity = T_ZAIKO.currentquantity + EXCLUDED.quantity,
                kousinnichiji = EXCLUDED.tourokunichiji;
                ";

            try
            {
                return await _executor.ExecuteAsync(sql, shiire);
            }
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            {
                throw new InvalidOperationException("指定された商品コードまたは仕入先コードが存在していません", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("仕入登録と在庫更新の処理中にエラーが発生しました", ex);
            }
        }

        public async Task<List<ShiireModel>> SearchAsync(string dateFrom, string dateTo, string shohinCode)
        {
            // 常に真の条件を設け、WHERE句の動的構築を容易にする
            var sql = @" SELECT * FROM T_SHIIRE WHERE 1 = 1 ";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(dateFrom))
            {
                sql += " AND shiiresakinengappi >= @DateFrom";
                parameters.Add("@DateFrom", dateFrom);
            }

            if (!string.IsNullOrEmpty(dateTo))
            {
                sql += " AND shiiresakinengappi <= @DateTo ";
                parameters.Add("@DateTo", dateTo);
            }

            if (!string.IsNullOrEmpty(shohinCode))
            {
                sql += "AND shohincode LIKE @ShohinCode ";
                parameters.Add("@ShohinCode", $"%{shohinCode}%");
            }
            sql += " ORDER BY shiiresakinengappi DESC, shohincode ASC;";

            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    var result = await _executor.QueryAsync<ShiireModel>(connection, sql, parameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("仕入伝票の検索中にエラーが発生しました。", ex);
            }
        }
        public async Task<ShiireModel?> GetByDateAndCodeAsync(string date, string code)
        {
            const string sql = @"
                SELECT * FROM 
                T_SHIIRE WHERE 
                shiiresakinengappi = @ShiireNengappi 
                AND
                shohincode = @ShohinCode;";

            var parameters = new { ShiireNengappi = date, ShohinCode = code };
            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    return await _executor.QueryFirstOrDefaultAsync<ShiireModel>(connection, sql, parameters);
                }
            }

            catch (Exception ex)
            {
                throw new ApplicationException("仕入伝票の単一取得中にエラーが発生しました。", ex);
            }
        }


        public async Task<int> UpdateAsync(ShiireModel shiire)
        {
            const string getOldQuantitySql = @" 
            SELECT quantity FROM T_SHIIRE 
            WHERE shiiresakinengappi = @ShiireNengappi 
            AND
            shohincode = @ShohinCode;";

            const string updateShiireSql = @"
            UPDATE T_SHIIRE SET
            shiiresakinengappi = @ShiireNengappi,
            shohincode = @ShohinCode,
            shiiresaki_code = @ShiiresakiCode, 
            quantity = @Quantity, 
            shiirene = @Shiirene 
            WHERE
            shiiresakinengappi = @ShiireNengappi 
            AND 
            shohincode = @ShohinCode;";

            const string updateZaikoSql = @"
            UPDATE T_ZAIKO SET 
            currentquantity = currentquantity + @QuantityDifference,
            kousinnichiji = NOW() 
            WHERE
            shohincode = @ShohinCode;";

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var oldQuantity = await _executor.QueryFirstOrDefaultAsync<int>(connection, getOldQuantitySql, shiire, transaction);
                        if (oldQuantity == default)
                        {
                            throw new InvalidOperationException("修正対象が見つかりません。");
                        }
                        var quantityDifference = shiire.Quantity - oldQuantity;
                        await connection.ExecuteAsync(updateShiireSql, shiire, transaction: transaction);

                        var zaikoParam = new
                        {
                            shiire.ShohinCode,
                            quantityDifference
                        };

                        await connection.ExecuteAsync(updateZaikoSql, zaikoParam, transaction: transaction);

                        transaction.Commit();
                        return 1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        if (ex is InvalidOperationException) throw;
                        throw new ApplicationException("仕入伝票の修正と在庫調整の処理中にエラーが発生しました。", ex);
                    }
                }
            }
        }
        public async Task<int> DeleteAsync(string date, string code, int quantity)
        {
            const string updateZaikoSql = @"
                UPDATE T_ZAIKO SET
                currentquantity = currentquantity - @Quantity, 

                kousinnichiji = NOW() 
                WHERE
                shohincode = @ShohinCode;";

            const string deleteShiireSql = @"
                DELETE FROM
                T_SHIIRE 
                WHERE
                shiiresakinengappi = @ShiireNengappi 
                AND
                shohincode = @ShohinCode;";

            var parameters = new
            {
                ShiireNengappi = date,
                ShohinCode = code,
                Quantity = quantity
            };

            using (var connection = _connectionFactory.CreateConnection())
            {
                connection.Open(); using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await connection.ExecuteAsync(updateZaikoSql, parameters, transaction: transaction);
                        int affectedRows = await
                            connection.ExecuteAsync(deleteShiireSql, parameters, transaction: transaction);
                        transaction.Commit();

                        return affectedRows;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new ApplicationException("仕入伝票の削除と在庫の払い戻しの処理中にエラーが発生しました。", ex);
                    }
                }
            }
        }
    }
}

