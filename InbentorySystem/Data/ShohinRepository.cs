using Dapper;
using InbentorySystem.Data.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace InbentorySystem.Data
{
    /// <summary>
    /// Npgaqlドライバを使用してPostgreSQLへの接続して（CRUD処理）を担当するリポジトリクラス
    /// </summary>
    public class ShohinRepository : IShohinRepository
    {
        // DB接続を生成するfactoryと,SQL実行するExecutorをDIで受け取る
        private readonly IDbConnectionFactory _factory;
        private readonly ISqlExecutor _sqlExecutor;

        public ShohinRepository(IDbConnectionFactory factory, ISqlExecutor sqlExecutor)
        {
            // フィールドの初期化
            _factory = factory;
            _sqlExecutor = sqlExecutor;

        }

        // -----------------------------------------
        // ①　検索処理　（GetAllAsync / SearchAsync）
        // -----------------------------------------

        /// <summary>
        /// 全ての商品データを取得する非同期メソッド
        /// </summary>
        /// <returns>全てのList</returns>
        public async Task<List<ShohinModel>> GetAllAsync()
        {
            // 全てのM_SHOHINの商品データを取得
            var sql = "SELECT * FROM m_shohin;";

            // usingを使うことで、メソッド終了時に非同期での接続が確実に閉じられ、メモリー解放
            // Dapperの使用
            using (var connection = _factory.CreateConnection())
            {
                var result = await _sqlExecutor.QueryAsync<ShohinModel>(connection, sql);
                return result.ToList();
            }
        }


        /// <summary>
        /// keywordを商品名（漢字orかな）を含む商品データ取得する非同期メソッド
        /// </summary>
        /// <param name="keyword">入力されたキーワード</param>
        /// <returns>ヒットしたList<ShohinModel></returns>
        public async Task<List<ShohinModel>> SearchByKeywordAsync(string keyword)
        {
            var sql = @"
                SELECT
                    *
                FROM
                    m_shohin M
                WHERE
                    shohin_mei_kanji ILIKE @Keyword OR shohin_mei_kana ILIKE @Keyword;";

            // Dapperはパラメータを直接オブジェクトに渡せる
            using (var connection = _factory.CreateConnection())
            {
                var param = new { Keyword = $"%{keyword}%" };

                var result = await _sqlExecutor.QueryAsync<ShohinModel>(connection, sql, param);
                return result.ToList();
            }
        }

        /// <summary>
        /// 指定された商品コードに基づいて単一の商品データ（在庫数を含む）を取得する
        /// </summary>
        /// <param name="shohinCode">検索対象の商品コード</param>
        /// <returns>ShohinModel or null</returns>
        public async Task<ShohinModel?> GetByCodeAsync(string shohinCode)
        {
            var sql = "SELECT * FROM m_shohin WHERE shohin_code = @ShohinCode;";

            using (var connection = _factory.CreateConnection())
            {
                var param = new { ShohinCode = shohinCode };
                return await _sqlExecutor.QueryFirstOrDefaultAsync<ShohinModel>(connection, sql, param);
            }
        }

        // ---------------------------
        // ②　登録処理（RegisterAsync）
        // ---------------------------

        /// <summary>
        /// 新しい商品をM_SHOHINとT_ZAIKOに登録する非同期メソッド(トランザクション必須)
        /// トランザクションを用いて、両テーブルへの登録処理を一貫して保証する。
        /// </summary>
        /// <param name="shohin">登録対象ShohinModel</param>
        public async Task<int> RegisterAsync(ShohinModel shohin)
        {
            var shohinSql = @"
                INSERT INTO m_shohin
                (shohin_code, shohin_mei_kanji, shohin_mei_kana, shiirene, urine, siiresaki_code)
                VALUES (@ShohinCode, @ShohinMeiKanji, @ShohinMeiKana, @Shiirene, @Urine, @ShiiresakiCode);";

            var zaikoSql = @"
                INSERT INTO t_zaiko
                (shohin_code, suryo, koushin_nichiji)
                VALUES (@ShohinCode, 0, NOW());";

            using (var connection = _factory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        int affectedRows = await _sqlExecutor.ExecuteAsync(shohinSql, shohin, transaction: transaction);

                        await _sqlExecutor.ExecuteAsync(zaikoSql, new {shohin.ShohinCode}, transaction:transaction);

                        transaction.Commit();
                        return affectedRows;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 商品コードの重複をチェックする非同期メソッド
        /// </summary>
        /// <param name="shohinCode">入力された商品コード</param>
        /// <returns>仮で、ロジックを成立させるため全部false</returns>
        public async Task<bool> CheckDuplicateCodeAsync(string shohinCode)
        {
            // SELECT COUNTは条件の一致するレコードだけを取得
            var sql = "SELECT COUNT(*) FROM m_shohin WHERE shohin_code = @ShohinCode";

            using (var connection = _factory.CreateConnection())
            {
                var param = new { ShohinCode = shohinCode };

                var count = await _sqlExecutor.QueryFirstOrDefaultAsync<long>(connection, sql, param);
                return count > 0;
            }
        }

        // ---------------------------
        // ③ 修正処理（UpdateAsync）
        // ---------------------------

        /// <summary>
        /// 既存の商品データをM_SHOHINとT_ZAIKOに修正する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">修正するShohinModel</param>
        public async Task<int> UpdateAsync(ShohinModel shohin)
        {
            // shohinテーブルの更新sql
            var shohinSql = @"
                UPDATE M_SHOHIN SET
                    shohin_mei_kanji = @ShohinMeiKanji,
                    shohin_mei_kana = @ShohinMeiKana,
                    shiirene = @Shiirene,
                    urine = @Urine,
                    siiresaki_code = @ShiiresakiCode
                WHERE
                    shohin_code = @ShohinCode;";

            // zaikoテーブルの更新sql
            var zaikoSql = @"
                UPDATE t_zaiko SET
                    koushin_nichiji = NOW()
                WHERE
                    shohin_code = @ShohinCode;";

            using (var connection = _factory.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var shohinParam = new
                        {
                            shohin.ShohinCode,
                            shohin.ShohinMeiKanji,
                            shohin.ShohinMeiKana,
                            shohin.Shiirene,
                            shohin.Urine,
                            ShiiresakiCode = (object?)shohin.ShiiresakiCode ?? DBNull.Value // Nullable対応
                        };

                        // DapperのExecuteAsyncにトランザクションを渡す
                        int affectedRows = await _sqlExecutor.ExecuteAsync(shohinSql, shohinParam, transaction: transaction);

                        var zaikoParam = new { shohin.ShohinCode };
                        await _sqlExecutor.ExecuteAsync(zaikoSql, zaikoParam, transaction: transaction);

                        transaction.Commit();
                        return affectedRows;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

            }
        }



        // ---------------------------
        // ④ 削除処理（DeleteAsync）
        // ---------------------------

        /// <summary>
        /// M_SHOHINとT_ZAIKOから指定された商品コードの商品データを削除する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">削除するShohinModel</param>
        public async Task DeleteAsync(string shohinCode)
        {
            // T_ZAIKOへのDELETE文(M_SHOHINは親レコードのため、T_ZAIKOが先)
            var zaikoSql = "DELETE FROM t_zaiko WHERE shohin_code = @Code;";

            // M_SHOHINへのDELETE文
            var shohinSql = "DELETE FROM m_shohin WHERE shohin_code = @Code;";

            var param = new {Code = shohinCode};

            using (var connection = _factory.CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await _sqlExecutor.ExecuteAsync(zaikoSql, param, transaction: transaction);
                        await _sqlExecutor.ExecuteAsync(shohinSql, param, transaction: transaction);
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 仕入先コードが存在するか確認するメソッド
        /// </summary>
        /// <param name="shiiresakiCode"></param>
        /// <returns>ヒットした仕入先コードの数</returns>
        public async Task<bool> CheckShiiresakiExistsAsync(string shiiresakiCode)
        {
            if (string.IsNullOrEmpty(shiiresakiCode))
            {
                return false;
            }

            var sql = "SELECT COUNT(*) FROM m_shiiresaki WHERE shiiresaki_code = @ShiiresakiCode";

            using (var connection = _factory.CreateConnection())
            {
                var param = new { ShiiresakiCode = shiiresakiCode };

                var count = await _sqlExecutor.QueryFirstOrDefaultAsync<long>(connection, sql, param);


                return count > 0;
            }
        }
    }
}
