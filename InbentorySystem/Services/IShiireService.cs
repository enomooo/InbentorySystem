using InbentorySystem.Data.Models;
using System.Collections.Generic;

namespace InbentorySystem.Services
{
    public interface IShiireService
    {
        // 検索結果を一時的に保持するプロパティ/メソッド(画面遷移用)
        void SetShiireList(List<ShiireModel> results);

        // 検索結果の件数に応じて、遷移先URIを決定するロジック
        List<ShiireModel> GetShiireList();

        string? DetermineNavigationUri(string dateKeyword, string codeKeyword, List<ShiireModel> results, string actionType);
    }
}
