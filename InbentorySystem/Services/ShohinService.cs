using System;
using System.Collections.Generic;
using InbentorySystem.Data.Models;

namespace InbentorySystem.Services
{
    public class ShohinService : IShohinService
    {
        public string DetermineNavigationUri(string keyword, List<ShohinModel> results)
        {
            // reusltsが null || 0 ならエラーメッセージのためstring.Emptyを返す
            if (results == null || results.Count == 0)
            {
                return string.Empty;
            }

            // キーワードなし -> 全一覧へ
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return "/shohin/list";
            }

            // キーワードあり、resultsが1以上
            else
            {
                var query = Uri.EscapeDataString(keyword);
                return $"/shohin/list?q={query}";
            }
        }
    }
}
