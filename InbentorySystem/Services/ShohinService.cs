using System;
using System.Collections.Generic;
using System.Diagnostics;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Services.Interfaces;

namespace InbentorySystem.Services
{
    /// <summary>
    /// 商品検索・修正・削除などの一時的な状態を保持するクラス
    /// UIとリポジトリ層の橋渡しを行うサービスクラス
    /// </summary>
    public class ShohinService : IShohinService
    {
        // 各種検索結果の一時保存
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

        /// <summary>
        /// 検索結果を取得
        /// </summary>
        /// <returns>検索結果</returns>
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
        /// <summary>
        /// 修正対象の商品を取得する
        /// </summary>
        /// <returns>修正対象</returns>
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
        /// 削除対象の商品データをrepository経由で削除するメソッド
        /// </summary>
        /// <param name="repository">削除対象の商品データのリポジトリ</param>
        /// <returns>削除処理</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task DeleteShohinAsync(IShohinRepository repository)
        {
            if (LastDeletedShohin == null)
                throw new InvalidOperationException("削除対象の商品が設定されていません");

            await repository.DeleteAsync(LastDeletedShohin.ShohinCode);
        }

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
