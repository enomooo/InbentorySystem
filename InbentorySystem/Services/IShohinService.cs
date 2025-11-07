using System;
using System.Collections.Generic;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Services
{
    public interface IShohinService
    {
        List<ShohinModel> GetShohinList();

        /// <summary>
        /// 状態とキーワードに基づく遷移先を動的に生成
        /// </summary>
        /// <param name="keyword">検索keyword</param>
        /// <returns>URI</returns>
        public string GetNavigationUri(string keyword);

        /// <summary>
        /// 修正対象保持
        /// </summary>
        /// <returns>修正対象</returns>
        public void SetLastEditedShohin(ShohinModel model);

        /// <summary>
        /// 検索結果保持
        /// </summary>
        /// <param name="results">検索結果</param>
        public void SetSearchResults(List<ShohinModel> results);

        /// <summary>
        /// 修正対象の商品を取得する
        /// </summary>
        /// <returns>修正対象</returns>
        public ShohinModel? GetLastEditedShohin();


    }
}



