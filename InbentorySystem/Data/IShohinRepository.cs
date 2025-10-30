using InbentorySystem.Data.Models;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InbentorySystem.Data
{
    public interface IShohinRepository
    {
        // 検索系メソッド
        Task<List<ShohinModel>> GetAllAsync();
        Task<ShohinModel?> GetByCodeAsync(string code);
        Task<List<ShohinModel>> SearchAsync(string keyword);

        // 登録・更新系メソッド
        Task<int> RegisterAsync(ShohinModel shohinModel);
        Task<int> UpdateAsync(ShohinModel shohinModel);

        // 削除系メソッド
        Task DeleteAsync(string code);

        // 存在・重複チェック系メソッド
        Task<bool> CheckDuplicateCodeAsync(string code);
        Task<bool> CheckShiiresakiExistsAsync(string shiiresakiCode);
    }
}
