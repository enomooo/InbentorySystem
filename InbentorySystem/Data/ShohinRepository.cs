// Npgaqlを使用してPostgreSQLへの接続して（CRUD処理）を担当するリポジトリクラス
using InbentorySystem.Data.Models;
using Npgsql;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace InbentorySystem.Data
{
    public class ShohinRepository
    {
        private readonly string _connectionSriting;

        // DI (Dependency Injection)で接続文字列を受け取るコンストラクタ
        public ShohinRepository(IConfiguration configuration)
        {
            _connectionSriting = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }


        // -----------------------------------------
        // ①　検索処理　（GetAllAsync / SearchAsync）
        // -----------------------------------------

        /// <summary>
        /// 全ての商品データを在庫数と共に取得する非同期メソッド
        /// </summary>
        /// <returns>全てのList<ShohinModel></returns>
        public async Task<List<ShohinModel>> GetAllAsync()
        {
            // T_ZAIKOとのLEFT JOINで全商品と在庫を取得するSQLのひな形
            var sql = @"
                SELECT
                    M.*, COALESCE(T.Suuryo, 0) AS Suuryo
                FROM
                    M_SHOHIN M
                LEFT JOIN
                    T_ZAIKO I ON M.ShohinCode = T.ShohhinCode;";
            // TODO: Npgsqlを使用してPostgreSQLに接続し、上記SQLを実行して結果をShohinModelのリストとして返す処理を実装
            return new List<ShohinModel>();
        }

        /// <summary>
        /// keywordを商品名に含む商品データを在庫数と共に取得する非同期メソッド
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>ヒットしたList<ShohinModel></returns>
        public async Task<List<ShohinModel>> SearchAsync(string keyword)
        {
            var sql = @"
                SELECT
                    M.*, COALESCE(T.Suuryo, 0) AS Suuryo
                FROM
                    M_SHOHIN M
                LEFT JOIN
                    T_ZAIKO T ON M.ShohinCode = T.ShohinCode
                WHERE
                    M.ShohinMeiKanji LIKE @Keyword;";
            // TODO: Npgsqlを使用してPostgreSQLに接続し,@Keywordパラメータを設定してSQLを実行する処理を実装
            return new List<ShohinModel>();
        }


        public async Task<ShohinModel> GetByCodeAsync(string shohinCode)
        {
            // TODO: Npgsqlを使用してPostgreSQLに接続し、指定されたshohinCodeの商品データを取得するsql処理を実装
            return new ShohinModel();
        }

        // ---------------------------
        // ②　登録処理（RegisterAsync）
        // ---------------------------

        /// <summary>
        /// 新しい商品をM_SHOHINとT_ZAIKOに登録する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">商品名(漢字orかな)</param>
        public async Task RegisterAsync(ShohinModel shohin)
        {
            // TODO: トランザクションを開始し、M_SHOHINとT_ZAIKOにINSERTする処理を実装
            // T_ZAIKOには初期在庫数0で登録すること
            await Task.CompletedTask;
        }

        /// <summary>
        /// 商品コードの重複をチェックする非同期メソッド
        /// </summary>
        /// <param name="shohinCode">入力された商品コード</param>
        /// <returns>仮で、ロジックを成立させるため全部false</returns>
        public async Task<bool> CheckDuplicateCodeAsync(string shohinCode)
        {
            // TODO: COUNT(*) を使用して、重複をチェックするSQLを実装
            return false;
        }

        // ---------------------------
        // ③ 修正処理（UpdateAsync）
        // ---------------------------

        /// <summary>
        /// 既存の商品データをM_SHOHINとT_ZAIKOに修正する非同期メソッド(トランザクション必須)
        /// </summary>
        /// <param name="shohin">商品名(漢字orかな)</param>
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
        /// <param name="shohinMei">商品名(漢字orかな)</param>
        public async Task DeleteAsync(string shohin)
        {
            await Task.CompletedTask;
        }
    }
}
