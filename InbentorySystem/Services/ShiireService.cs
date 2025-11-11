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
        /// <summary>
        /// 検索結果の一時保存
        /// </summary>
        public List<ShiireModel> SearchResults { get; private set; } = new();

        /// <summary>
        /// 検索条件の仕入年月の保持
        /// </summary>
        public string? LastDateFrom { get; private set; }

        /// <summary>
        /// 検索条件の商品コードの保持
        /// </summary>
        public string? LastCodeKeyword { get; private set; }

        /// <summary>
        /// 登録結果の保持
        /// </summary>
        public ShiireModel? LastRegisteredShiire { get; private set; }

        /// <summary>
        /// 修正結果の保持
        /// </summary>
        public ShiireModel? LastEditedShiire { get; private set; }

        /// <summary>
        /// 削除結果の保持
        /// </summary>
        public ShiireModel? LastDeletedShiire { get; private set; }

        /// <summary>
        /// 検索結果と意図に応じて遷移先URIを返す
        /// </summary>
        public string? DetermineNavigationUri(string dateFrom, string codeKeyword, List<ShiireModel> results, string actionType)
        {
            SearchResults = results;
            LastDateFrom = dateFrom;
            LastCodeKeyword = codeKeyword;

            if (results == null || results.Count == 0)
            {
                return null;
            }

            return "/shiirelist";
        }

        /// <summary>
        /// UIから検索結果をセット
        /// </summary>
        public void SetShiireList(List<ShiireModel> list)
        {
            SearchResults = list;
        }

        /// <summary>
        /// 検索結果を取得
        /// </summary>
        public List<ShiireModel> GetShiireList()
        {
            return SearchResults;
        }

        /// <summary>
        /// 登録結果を保持
        /// </summary>
        public void SetLastRegisteredShiire(ShiireModel model)
        {
            LastRegisteredShiire = model;
        }

        /// <summary>
        /// 修正結果を保持
        /// </summary>
        public void SetLastEditedShiire(ShiireModel model)
        {
            LastEditedShiire = model;
        }

        /// <summary>
        /// 削除結果を保持
        /// </summary>
        public void SetLastDeletedShiire(ShiireModel model)
        {
            LastDeletedShiire = model;
        }

        /// <summary>
        /// 全ての一時状態をクリア
        /// </summary>
        public void Clear()
        {
            SearchResults.Clear();
            LastDateFrom = null;
            LastCodeKeyword = null;
            LastRegisteredShiire = null;
            LastEditedShiire = null;
            LastDeletedShiire = null;
        }
    }
}
