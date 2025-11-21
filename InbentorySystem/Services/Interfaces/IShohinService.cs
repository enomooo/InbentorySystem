using System.Collections.Generic;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Services.Interfaces
{
    /// <summary>
    /// 商品検索・登録・修正・削除などの一時的な状態を保持するサービスのインターフェース
    /// </summary>
    public interface IShohinService
    {
        // --- 検索関連 ---

        /// <summary>
        /// 商品一覧（全件取得結果）を返す
        /// </summary>
        List<ShohinModel> GetShohinList();

        /// <summary>
        /// 商品一覧（全件取得結果）を保持する
        /// </summary>
        void SetShohinList(List<ShohinModel> list);

        /// <summary>
        /// 検索結果を保持する
        /// </summary>
        void SetSearchResults(List<ShohinModel> results);

        /// <summary>
        /// 検索結果を取得する
        /// </summary>
        List<ShohinModel> GetSearchResults();

        /// <summary>
        /// 状態とキーワードに基づく遷移先を生成
        /// </summary>
        string GetNavigationUri(string keyword);


        // --- 登録関連 ---

        /// <summary>
        /// 最後に登録された商品を保持する
        /// </summary>
        void SetLastRegisteredShohin(ShohinModel shohin);

        /// <summary>
        /// 最後に登録された商品を取得する
        /// </summary>
        ShohinModel? GetLastRegisteredShohin();

        /// <summary>
        /// 登録済み商品キャッシュをクリアする
        /// </summary>
        void ClearLastRegisteredShohin();


        // --- 修正関連 ---

        /// <summary>
        /// 修正対象を保持する
        /// </summary>
        void SetLastEditedShohin(ShohinModel model);

        /// <summary>
        /// 修正対象を取得する
        /// </summary>
        ShohinModel? GetLastEditedShohin();

        /// <summary>
        /// 修正対象プロパティ
        /// </summary>
        ShohinModel? LastEditedShohin { get; }


        // --- 更新関連 ---

        /// <summary>
        /// 更新後の商品を保持する
        /// </summary>
        void SetLastUpdatedShohin(ShohinModel model);

        /// <summary>
        /// 更新後の商品を取得する
        /// </summary>
        ShohinModel? GetLastUpdatedShohin();

        /// <summary>
        /// 更新後の商品キャッシュをクリアする
        /// </summary>
        void ClearLastUpdatedShohin();


        // --- 削除関連 ---

        /// <summary>
        /// 削除対象を保持する
        /// </summary>
        void SetLastDeletedShohin(ShohinModel model);

        /// <summary>
        /// 削除対象を取得する
        /// </summary>
        ShohinModel? GetLastDeletedShohin();

        /// <summary>
        /// 削除対象プロパティ
        /// </summary>
        ShohinModel? LastDeletedShohin { get; }

        /// <summary>
        /// クエリに基づいて商品データを検索するロジックをリポジトリに委譲
        /// </summary>
        Task<List<ShohinModel>> SearchShohinAsync(string query);

    }
}
