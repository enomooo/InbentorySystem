using InbentorySystem.Data.Models;

namespace InbentorySystem.Infrastructure.Interfaces
{

    /// <summary>
    /// 仕入データの登録・検索・取得・修正・削除を扱うリポジトリインターフェイス
    /// </summary>
    public interface IShiireRepository
    {
        /// <summary>
        /// 新規仕入登録（在庫更新も含む）
        /// </summary>
        /// <param name="shiire">仕入モデル</param>
        /// <returns>影響を受けた行数</returns
        Task<int> RegisterAsync(ShiireModel shiire);

        /// <summary>
        /// 月単位検索（年月＋商品コード）
        /// </summary>
        /// <param name="year">検索対象の年</param>
        /// <param name="month">検索対象の月</param>
        /// <param name="shohinCode">商品コード（部分一致可）</param>
        /// <returns>該当する仕入リスト</returns>
        Task<List<ShiireModel>> SearchByMonthAsync(int year, int month, string? shohinCode);

        /// <summary>
        /// 日付検索（年月日＋商品コード）
        /// </summary>
        /// <param name="date">検索対象の日付</param>
        /// <param name="shohinCode">商品コード（部分一致可）</param>
        /// <returns>該当する仕入リスト</returns>
        public  Task<List<ShiireModel>> SearchByDateAsync(DateTime date, string shohinCode);

        /// <summary>
        /// 単一取得（日付＋商品コード）
        /// </summary>
        /// <param name="date">仕入日付</param>
        /// <param name="code">商品コード</param>
        /// <returns>該当する仕入モデル（存在しない場合はnull）</returns>
        Task<ShiireModel?> GetByDateAndCodeAsync(DateTime date, string code);

        /// <summary>
        /// 修正処理（仕入伝票＋在庫調整）
        /// </summary>
        /// <param name="shiire">修正対象の仕入モデル</param>
        /// <returns>影響を受けた行数</returns>
        Task<int> UpdateAsync(ShiireModel shiire);

        /// <summary>
        /// 削除処理（仕入伝票＋在庫調整）
        /// </summary>
        /// <param name="date">仕入日付</param>
        /// <param name="code">商品コード</param>
        /// <param name="quantity">削除対象数量</param>
        /// <returns>影響を受けた行数</returns>
        Task<int> DeleteAsync(DateTime date, string code, int quantity);
    }
}
