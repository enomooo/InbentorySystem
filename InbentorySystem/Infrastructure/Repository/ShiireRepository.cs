using Dapper;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Infrastructure.Repository
{
    public class ShiireRepository : IShiireRepository
    {
        // 依存性の注入 (IDbConnectionFactoryとISqlExecutorは既存のものを使用)
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ISqlExecutor _executor;

        /// <summary>
        /// 仕入リポジトリのインスタンス生成
        /// </summary>
        /// <param name="connectionFactory">DB接続生成するためのファクトリ</param>
        /// <param name="executor">SQL実行のためのユーティリティ</param>
        public ShiireRepository(IDbConnectionFactory connectionFactory, ISqlExecutor executor)
        {
            _connectionFactory = connectionFactory;
            _executor = executor;
        }

        /// <summary>
        /// 月単位検索（年月＋商品コード）
        /// </summary>
        /// <param name="year">入力年</param>
        /// <param name="month">入力月</param>
        /// <param name="shohinCode">入力商品コード</param>
        /// <returns>検索結果</returns>
        public async Task<List<ShiireModel>> SearchByMonthAsync(int year, int month, string? shohinCode)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var sql = @"
                   SELECT 
                        shiire_no AS ShiireNo,
                        shiire_bi AS ShiireNengappi,
                        shohin_code AS ShohinCode,
                        siiresaki_code AS ShiiresakiCode,
                        suryo AS Quantity
                    FROM t_shiire
                    WHERE shiire_bi >= @StartDate
                    AND shiire_bi < @EndDate";

            var parameters = new DynamicParameters();
            parameters.Add("@StartDate", startDate);
            parameters.Add("@EndDate", endDate);

            if (!string.IsNullOrEmpty(shohinCode))
            {
                sql += " AND shohin_code = @ShohinCode";
                parameters.Add("@ShohinCode", shohinCode);
            }

            sql += " ORDER BY shiire_bi DESC, shohin_code ASC;";

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await _executor.QueryAsync<ShiireModel>(connection, sql, parameters);
                return result.ToList();
            }

            catch (Exception ex)
            {
                throw new ApplicationException("仕入伝票の月単位検索中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// 日付検索（年月日＋商品コード）
        /// </summary>
        /// <param name="date">入力された日付</param>
        /// <param name="shohinCode">入力された商品コード</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<List<ShiireModel>> SearchByDateAsync(DateTime date, string shohinCode)
        {
            var sql = @" 
                SELECT shiire_no,
                        shiire_bi AS ShiireNengappi,
                shohin_code AS ShohinCode,
                siiresaki_code AS ShiiresakiCode,
                suryo AS Quantity
            FROM t_shiire
            WHERE shiire_bi = @Date";
            var parameters = new DynamicParameters();
            parameters.Add("@Date", date.Date);


            if (!string.IsNullOrEmpty(shohinCode))
            {
                sql += " AND shohin_code LIKE @ShohinCode";
                parameters.Add("@ShohinCode", $"%{shohinCode}%");
            }

            sql += " ORDER BY Shiire_bi DESC, shohin_code ASC;";

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var result = await _executor.QueryAsync<ShiireModel>(connection, sql, parameters);
                return result.ToList();
            }

            catch (Exception ex)
            {
                throw new ApplicationException("仕入伝票の月単位検索中にエラーが発生しました。", ex);
            }
        }

        /// <summary>
        /// DBにトランザクションでT_SHIIREとT_ZAIKOに登録処理
        /// </summary>
        /// <param name="shiire">新規登録するmodel</param>
        /// <returns></returns>
        public async Task<int> RegisterAsync(ShiireModel shiire)
        {
            shiire.Tourokunichiji = DateTime.Now;

            
            const string sql = @"
        INSERT INTO t_shiire (shiire_bi, shohin_code, siiresaki_code, suryo)
        VALUES (@ShiireNengappiParam, @ShohinCode, @ShiiresakiCode, @Quantity);

        INSERT INTO t_zaiko (shohin_code, suryo, koushin_nichiji)
        VALUES (@ShohinCode, @Quantity, @Tourokunichiji)
        ON CONFLICT (shohin_code)
        DO UPDATE SET
            suryo = t_zaiko.suryo + EXCLUDED.suryo, 
            koushin_nichiji = EXCLUDED.koushin_nichiji;
        ";

            var parameters = new
            {
                shiire.ShiireBi,
                shiire.ShohinCode,
                shiire.ShiiresakiCode,
                shiire.Quantity,
                shiire.Tourokunichiji
            };

            try
            {
                return await _executor.ExecuteInTransactionAsync(sql, parameters);
            }
            // 23503はforeignキー制約違反
            catch (Npgsql.PostgresException ex) when (ex.SqlState == "23503")
            {
                throw new InvalidOperationException("指定された商品コードまたは仕入先コードが存在していません", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("仕入登録と在庫更新の処理中にエラーが発生しました", ex);
            }
        }

        /// <summary>
        /// DBにアクセスして検索処理
        /// </summary>
        /// <param name="dateFrom">何日～</param>
        /// <param name="dateTo">何日まで</param>
        /// <param name="shohinCode">商品コード</param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<List<ShiireModel>> SearchAsync(string dateFrom, string dateTo, string shohinCode)
        {
            // 常に真の条件を設け、WHERE句の動的構築を容易にする
            var sql = @" SELECT * FROM T_SHIIRE WHERE 1 = 1 ";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(dateFrom))
            {
                sql += " AND shiire_bi >= @DateFrom";
                parameters.Add("@DateFrom", dateFrom);
            }

            if (!string.IsNullOrEmpty(dateTo))
            {
                sql += " AND shiire_bi <= @DateTo ";
                parameters.Add("@DateTo", dateTo);
            }

            if (!string.IsNullOrEmpty(shohinCode))
            {
                sql += "AND shohincode LIKE @ShohinCode ";
                parameters.Add("@ShohinCode", $"%{shohinCode}%");
            }
            sql += " ORDER BY shiire_bi DESC, shohincode ASC;";

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

        /// <summary>
        /// 日付と商品コードから仕入検索（修正と削除）
        /// </summary>
        /// <param name="date">年月日</param>
        /// <param name="code">商品コード</param>
        /// <returns>該当したShiireModel</returns>
        /// <exception cref="ApplicationException"></exception>
        public async Task<ShiireModel?> GetByDateAndCodeAsync(string date, string code)
        {
            const string sql = @"
                SELECT * FROM 
                T_SHIIRE WHERE 
                shiire_bi = @ShiireNengappi 
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

        /// <summary>
        /// トランザクション処理でDB修正メソッド
        /// </summary>
        /// <param name="shiire">選択されたShiireModel</param>
        /// <returns>修正結果</returns>
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

        /// <summary>
        /// トランザクション削除メソッド
        /// </summary>
        /// <param name="date">入力された日付</param>
        /// <param name="code">商品コード</param>
        /// <param name="quantity">数量</param>
        /// <returns>削除結果</returns>
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

