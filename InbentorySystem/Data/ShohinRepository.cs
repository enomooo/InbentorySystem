// PostgreSQLへの接続にNpgsqlを使用しています。
using Npgsql;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Data
{
    /// <summary>
    /// PostgreSQLのM_SHOHINテーブルにアクセスするリポジトリクラス
    /// </summary>
    public class ShohinRepository
    {
        // 依存性の注入を使用してNpgsqlConnectionを受け取るコンストラクタ
        private readonly NpgsqlConnection _connection;
        public ShohinRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// 商品一覧を非同期で取得する
        /// </summary>
        /// <returns>shohinList</returns>
        public async Task<List<ShohinModel>> GetAllAsync()
        {
            var shohinList = new List<ShohinModel>();

            var sql = @"
                SELECT 
                    shohin_code, 
                    shohin_kanji_mei, 
                    shohin_kana_mei, 
                    siiire_kakaku, 
                    hanbai_kakaku 
                FROM 
                    M_SHOHIN;
            ";

            // DB接続を開く
            await _connection.OpenAsync();

            // SQLコマンドを実行し, 結果を1行ずつ読み取る
            await using (var cmd = new NpgsqlCommand(sql, _connection))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                // 次の行が存在する限りループして、データを取得する
                while (await reader.ReadAsync())
                {
                    shohinList.Add(new ShohinModel
                    {
                        ShohinCode = reader.GetString(0),
                        ShohinKanjiMei = reader.GetString(1),
                        ShohinKanaMei = reader.GetString(2),
                        Siiirekakaku = reader.GetInt32(3),
                        HanbaiKakaku = reader.GetInt32(4)
                    });
                }
            }
            // DB接続を閉じる
            await _connection.CloseAsync();
            return shohinList;
        }
    }
}
