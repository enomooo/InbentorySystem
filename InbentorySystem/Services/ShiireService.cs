using InbentorySystem.Data.Models;
using InbentorySystem.Services.Interfaces;
using System.Collections.Generic;

namespace InbentorySystem.Services
{
    /// <summary>
    /// 仕入機能に関する状態管理と遷移制御を担うサービス。
    /// 主に検索・登録・修正・削除後の結果表示に必要なデータを保持する。
    /// </summary>
    public class ShiireService : IShiireService
    {

        // 最後に検索した商品コードキーワード（検索条件の記録用）
        public string? LastCodeKeyword { get; private set; }

        // 最後に登録された仕入データ
        public ShiireModel? LastRegisteredShiire { get; private set; }

        // 最後に修正された仕入データ
        public ShiireModel? LastEditedShiire { get; private set; }
        public int? LastEditBeforeZaikoQuantity { get; private set; }
        public int? LastEditAfterZaikoQuantity { get; private set; }
        public ShiireModel? LastEditOriginalShiire { get; private set; }
        // 最後に削除された仕入データ
        public ShiireModel? LastDeletedShiire { get; private set; }

        /// <summary>
        /// 検索結果と意図に応じて遷移先URIを返す
        /// </summary>
        public string? DetermineNavigationUri(string dateFrom, string codeKeyword, List<ShiireModel> results, string actionType)
        {
            _searchResults = results;
            LastCodeKeyword = codeKeyword;

            if (results == null || results.Count == 0)
            {
                return null;
            }
            return "/shiirelist";
        }

        private List<ShiireModel> _searchResults = new();

        /// <summary>
        /// 検索結果リストをセットする
        /// </summary>
        /// <param name="list">検索結果リスト</param>
        public void SetShiireList(List<ShiireModel> list)
        {
            _searchResults = list;
        }

        public List<ShiireModel> SearchResults => _searchResults;

        /// <summary>
        /// 現在保持している仕入れリストを返す
        /// </summary>
        /// <returns>現在保持している仕入れリスト</returns>
        public List<ShiireModel> GetShiireList()
        {
            return _searchResults;
        }

        /// <summary>
        /// 検索結果をセットする
        /// </summary>
        /// <param name="results">検索結果</param>
        public void SetSearchResults(List<ShiireModel> results)
        {
            _searchResults = results;
        }

        public List<ShiireModel> GetSearchResults()
        {
            return _searchResults;
        }

        public void SetLastRegisteredShiire(ShiireModel model)
        {
            LastRegisteredShiire = model;
        }

        public ShiireModel? GetLastRegisteredShiire()
        {
            return LastRegisteredShiire;
        }

        public void SetLastEditedShiire(ShiireModel model)
        {
            LastEditedShiire = model;
        }

        /// <summary>
        /// 修正前後のデータ
        /// </summary>
        /// <param name="before">修正前のデータ</param>
        /// <param name="after">修正後のデータ</param>
        public void SetLastEditResults(ShiireModel original, ShiireModel updated, int beforeZaiko, int afterZaiko)
        {
            LastEditOriginalShiire = original;
            LastEditedShiire = updated;
            LastEditBeforeZaikoQuantity = beforeZaiko;
            LastEditAfterZaikoQuantity = afterZaiko;
        }

        public ShiireModel? GetLastEditedShiire()
        {
            return LastEditedShiire;
        }

        public void SetLastEditResult(ShiireModel model)
        {
            LastEditedShiire = model;
        }

        public ShiireModel? GetLastEditResult()
        {
            return LastEditedShiire;
        }

        public void SetLastDeleteResult(ShiireModel model)
        {
            LastDeletedShiire = model;
        }

        public ShiireModel? GetLastDeleteResult()
        {
            return LastDeletedShiire;
        }

        public void SetLastDeletedShiire(ShiireModel model) => LastDeletedShiire = model;
        public ShiireModel? GetLastDeletedShiire() => LastDeletedShiire;

        public void Clear()
        {
            _searchResults.Clear();
            LastCodeKeyword = null;
            LastRegisteredShiire = null;
            LastEditedShiire = null;
            LastDeletedShiire = null;

            LastEditAfterZaikoQuantity = null;
            LastEditBeforeZaikoQuantity = null;
            LastEditOriginalShiire = null;
        }
    }
}
