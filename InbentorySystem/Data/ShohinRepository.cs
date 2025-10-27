using InbentorySystem.Data.Models;
using Npgsql;
using System.Data;
using System.Linq.Expressions;
using System.Net.WebSockets;

namespace InbentorySystem.Data
{
    /// <summary>
    /// Npgaqlドライバを使用してPostgreSQLへの接続して（CRUD処理）を担当するリポジトリクラス
    /// </summary>
    public class ShohinRepository
    {
        // データベース接続文字列を保持するプライベートなフィールド
        private readonly string _connectionString;

        /// <summary>
        /// コンストラクタ：DIにより、アプリケーションの設定情報（aoosettings.Development.jsonのDefaultConnection）
        /// を受け取り、データベースへの接続文字列を初期化します
        /// </summary>
        /// <param name="configuration">アプリケーションの設定情報</param>
        public ShohinRepository(IConfiguration configuration)
        {
            // 接続文字列が見つからないなら、起動時に例外をスロー
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }


        // -----------------------------------------
        // ①　検索処理　（GetAllAsync / SearchAsync）
        // -----------------------------------------

        /// <summary>
        /// 全ての商品データを取得する非同期メソッド
        /// </summary>
        /// <returns>全てのList<ShohinModel></returns>
        public async Task<List<ShohinModel>> GetAllAsync()
        {
            // 全てのM_SHOHINの商品データを取得
            var sql = "SELECT * FROM M_SHOHIN;";

            var shohinList = new List<ShohinModel>();

            // usingを使うことで、メソッド終了時に非同期での接続が確実に閉じられ、メモリー解放
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            // todo:関数化候補　実行するSQL(sql)と使用する接続(conn)を紐づけたNpgsqlCommandを作成
            await using var cmd = new NpgsqlCommand(sql, conn);

            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                shohinList.Add(MapToShohinModel(reader));
            }
            return shohinList;
        }


        /// <summary>
        /// keywordを商品名（漢字orかな）を含む商品データ取得する非同期メソッド
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>ヒットしたList<ShohinModel></returns>
        public async Task<List<ShohinModel>> SearchAsync(string keyword)
        {
            var sql = @"
                SELECT
                    *
                FROM
                    M_SHOHIN M
                WHERE
                    -- Postgre_SQLではILikeを使用すると大文字/小文字を区別しない検索が可能
                    ShohinMeiKanji ILIKE @Keyword OR ShohinMeiKana ILIKE @Keyword;";
            var shohinList = new List<ShohinModel>();

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, conn);

            // @Keywordパラメータを設定(部分一致検索のため、前後に%を付与)
            cmd.Parameters.AddWithValue("keyword", $"%{keyword}%");

            // SQLを実行し、結果を読み取る
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                shohinList.Add(MapToShohinModel(reader));
            }
            return shohinList;
        }

        private ShohinModel MapToShohinModel(NpgsqlDataReader reader)
        {
            return new ShohinModel
            {
                ShohinCode = reader["ShohinCode"].ToString() ?? string.Empty,
                ShohinMeiKanji = reader["ShohinMeiKanji"].ToString() ?? string.Empty,
                ShohinMeiKana = reader["ShohinMeiKana"].ToString() ?? string.Empty,
                ShiireKakaku = reader.GetInt32("ShiireKakaku"),
                HanbaiKakaku = reader.GetInt32("HanbaiKakaku"),
                ShiiresakiCode = reader["ShiiresakiCode"].ToString() ?? string.Empty,
            };
        }

        /// <summary>
        /// 指定された商品コードに基づいて単一の商品データ（在庫数を含む）を取得する
        /// </summary>
        /// <param name="shohinCode">検索対象の商品コード</param>
        /// <returns>ShohinModel or null</returns>
        public async Task<ShohinModel> GetByCodeAsync(string shohinCode)
        {
            var sql = "SELECT * FROM M_SHOHIN WHERE ShohinCode = @ShohinCode;";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("ShohinCode", shohinCode);

            await using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapToShohinModel(reader);
            }
            // 該当商品がない場合はnullで返す
            return null;
        }

        // ---------------------------
        // ②　登録処理（RegisterAsync）
        // ---------------------------

        /// <summary>
        /// 新しい商品をM_SHOHINとT_ZAIKOに登録する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">入力したShohinModel</param>
        public async Task RegisterAsync(ShohinModel shohin)
        {
            var shohinSql = @"
                INSERT INTO M_SHOHIN
                (ShohinCode, ShohinMeiKanji, ShohinMeiKana, ShiireKakaku, HanbaiKakaku, ShiiresakiCode)
                VALUES (@Code, @Kanji, @Kana, @Shiire, @Hanbai, @Shiiresaki);";

            var zaikoSql = @"
                INSERT INTO T_ZAIKO
                (ShohinCode, Suuryo, KoushinNuchiji)
                VALUES (@Code, 0, NOW());";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var transaction = await conn.BeginTransactionAsync();

            try
            {
                await using (var shohinCmd = new NpgsqlCommand(shohinSql, conn, transaction))
                {
                    shohinCmd.Parameters.AddWithValue("Code", shohin.ShohinCode);
                    shohinCmd.Parameters.AddWithValue("Kanji", shohin.ShohinMeiKanji);
                    shohinCmd.Parameters.AddWithValue("Kana", shohin.ShohinMeiKana);
                    shohinCmd.Parameters.AddWithValue("Shiire", shohin.ShiireKakaku);
                    shohinCmd.Parameters.AddWithValue("Hanbai", shohin.HanbaiKakaku);
                    shohinCmd.Parameters.AddWithValue("Shiiresaki", shohin.ShiiresakiCode);

                    await shohinCmd.ExecuteNonQueryAsync();
                }

                await using (var zaikoCmd = new NpgsqlCommand(zaikoSql, conn, transaction))
                {
                    zaikoCmd.Parameters.AddWithValue("Code", shohin.ShohinCode);
                    await zaikoCmd.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
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
            var sql = "SELECT COUNT(*) FROM M_SHOHIN WHERE ShohinCode = @ShohinCode";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("ShohinCode", shohinCode);

            object? result = await cmd.ExecuteScalarAsync();
            var count = result as long? ?? 0L;
            return count > 0;
        }

        // ---------------------------
        // ③ 修正処理（UpdateAsync）
        // ---------------------------

        /// <summary>
        /// 既存の商品データをM_SHOHINとT_ZAIKOに修正する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">修正するShohinModel</param>
        public async Task UpdateAsync(ShohinModel shohin)
        {
            // TODO: トランザクションを開始し、M_SHOHINとT_ZAIKOにUPDATEする処理を実装
            await Task.CompletedTask;
        }

        // ---------------------------
        // ④ 削除処理（DeleteAsync）
        // ---------------------------

        /// <summary>
        /// M_SHOHINとT_ZAIKOから指定された商品コードの商品データを削除する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">削除するShohinModel</param>
        public async Task DeleteAsync(string shohin)
        {
            await Task.CompletedTask;
        }
    }
}
