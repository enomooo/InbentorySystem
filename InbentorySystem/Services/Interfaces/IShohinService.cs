using System;
using System.Collections.Generic;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Services.Interfaces
{
    public interface IShohinService
    {
        // --- 検索関連 ---
        List<ShohinModel> GetShohinList();

        /// <summary>
        /// 検索結果保持
        /// </summary>
        /// <param name="results">検索結果</param>
        public void SetSearchResults(List<ShohinModel> results);

        /// <summary>
        /// 状態とキーワードに基づく遷移先を動的に生成
        /// </summary>
        /// <param name="keyword">検索keyword</param>
        /// <returns>URI</returns>
        public string GetNavigationUri(string keyword);

        // --- 登録関連 ---

        /// <summary>
        /// 最後に登録された商品モデルを保持する
        /// </summary>
        public void SetLastRegisteredShohin(ShohinModel shohin);

        /// <summary>
        /// 最後に登録された商品モデルを取得する
        /// </summary>
        public ShohinModel? GetLastRegisteredShohin();

        /// <summary>
        /// 最後に登録された商品モデルのキャッシュをクリアする
        /// </summary>
        public void ClearLastRegisteredShohin();


        // --- 修正関連 ---
        ShohinModel? LastEditedShohin { get; }

        /// <summary>
        /// 修正対象保持
        /// </summary>
        public void SetLastEditedShohin(ShohinModel model);

        /// <summary>
        /// 修正対象の商品を取得する
        /// </summary>
        public ShohinModel? GetLastEditedShohin();

        // --- 削除関連 ---

        /// <summary>
        /// 削除対象の商品を保持する
        /// </summary>
        public void SetLastDeletedShohin(ShohinModel model);

        ShohinModel? LastDeletedShohin { get; }
    }
}
