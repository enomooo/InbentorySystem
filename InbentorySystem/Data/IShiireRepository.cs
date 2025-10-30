using InbentorySystem.Data.Models;

namespace InbentorySystem.Data
{
    public interface IShiireRepository
    {
        // 仕入伝票の登録と在庫の更新を伴う
        Task<int> RegisterAsync(ShiireModel shiire);

        // 仕入伝票の検索
        Task<List<ShiireModel>> SearchAsync(string dateFrom, string dateTo, string shohinCode);

        // 特定の仕入伝票を取得
        Task<ShiireModel> GetByDateAndCodeAsync(string date, string code);

        // 修正（数量や仕入先コードの変更、在庫の調整を伴う）
        Task<int> UpdateAsync(ShiireModel shiire);

        // 削除　(仕入伝票の削除と在庫の調整を伴う)
        Task<int> DeleteAsync(string date, string code, string quantity);
    }
}
