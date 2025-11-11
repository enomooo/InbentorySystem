using InbentorySystem.Data.Models;

namespace InbentorySystem.Infrastructure.Interfaces
{
    /// <summary>
    /// 仕入データの登録・検索・取得・修正・削除を扱うリポジトリインターフェイス
    /// </summary>
    public interface IShiireRepository
    {
        Task<int> RegisterAsync(ShiireModel shiire);
        Task<List<ShiireModel>> SearchAsync(string dateFrom, string dateTo, string shohinCode);
        Task<ShiireModel?> GetByDateAndCodeAsync(string date, string code);
        Task<int> UpdateAsync(ShiireModel shiire);
        Task<int> DeleteAsync(string date, string code, int quantity);
    }
}
