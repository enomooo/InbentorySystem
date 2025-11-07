using System.Threading.Tasks;

namespace InbentorySystem.Services
{
    public interface IZaikoService
    {
        /// <summary>
        /// 指定された商品コードの在庫数を加算、減算する
        /// </summary>
        /// <param name="shohinCode">商品コード</param>
        /// <param name="quantityDiff">加減算する数量</param>
        Task UpdateQuantityAsync(string shohinCode, int quantityDiff);

        /// <summary>
        /// 現在の在庫数を取得
        /// </summary>
        /// <param name="shohinCode">商品コード</param>
        /// <returns>現在の数量</returns>
        Task<int> GetCurrentQuantityAsync(string shohinCode);

        /// <summary>
        /// 指定された数量の在庫が十分にあるか判定
        /// </summary>
        /// <param name="shohinCode">商品コード</param>
        /// <param name="requiredQuantity">要求された数量</param>
        /// <returns>在庫が足りていればtrue</returns>
        Task<bool> IsStockSufficientAsync(string shohinCode, int requiredQuantity);
    }

}