using System;
using System.Collections.Generic;
using System.Diagnostics;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Services
{

    public class ShohinService : IShohinService
    {
        private List<ShohinModel> _searchResults = new();
        private ShohinModel? _lastEditedShohin;
        private ShohinModel? _lastDeletedShohin;
        private List<ShohinModel> _shohinList = new();

        /// <summary>
        /// 商品一覧の保持
        /// </summary>
        /// <returns>商品一覧</returns>
        public List<ShohinModel> GetShohinList()
        {
            return _shohinList;
        }

        /// <summary>
        /// 検索結果の保持
        /// </summary>
        /// <param name="results">検索結果</param>
        public void SetSearchResults(List<ShohinModel> results)
        {
            _searchResults = results;
        }

        public List<ShohinModel> GetSearchResults()
        {
            return _searchResults;
        }

        /// <summary>
        /// 修正対象の保持
        /// </summary>
        /// <returns>修正対象</returns>
        public void SetLastEditedShohin(ShohinModel model)
        {
            _lastEditedShohin = model;
        }
        public ShohinModel? LastEditedShohin => _lastEditedShohin;

        public ShohinModel? GetLastEditedShohin()
        {
            return _lastEditedShohin;
        }

        /// <summary>
        /// 修正対象の商品データをrepository経由で更新するメソッド
        /// </summary>
        /// <param name="repository">更新する商品データのインスタンス</param>
        /// <returns>更新処理</returns>
        public async Task UpdateShohinAsync(IShohinRepository repository)
        {
            if (LastEditedShohin == null)
                throw new InvalidOperationException("修正対象の商品が設定されていません");

            await repository.UpdateAsync(LastEditedShohin);
        }

        /// <summary>
        /// 削除対象の保持
        /// </summary>
        public void SetLastDeletedshohin(ShohinModel model)
        {
            _lastDeletedShohin = model;
        }
        public ShohinModel? LastDeletedShohin => _lastDeletedShohin;

        /// <summary>
        /// 状態とキーワードに基づく遷移先を動的に生成
        /// </summary>
        /// <param name="keyword">検索keyword</param>
        /// <returns>URI</returns>
        public string GetNavigationUri(string keyword)
        {
            if (_searchResults == null || _searchResults.Count == 0)
                return string.Empty;

            return string.IsNullOrWhiteSpace(keyword)
                ? "/shohin/list"
                : $"/shohin/list?q={Uri.EscapeDataString(keyword)}";
        }
    }
}
