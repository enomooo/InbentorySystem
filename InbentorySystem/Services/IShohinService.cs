using System;
using System.Collections.Generic;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Services
{
    public interface IShohinService
    {
        /// <summary>
        /// 検索結果の件数に基づき、次に遷移すべきURIを決定する
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public string DetermineNavigationUri(string keyword, List<ShohinModel> results);
    }
}



