using InbentorySystem.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace InbentorySystem.Services
{
    public class ShiireService : IShiireService
    {
        private static List<ShiireModel> _currentShiireList = new();

        /// <summary>
        /// 検索結果を静的リストに設定する
        /// </summary>
        /// <param name="results">検索結果</param>
        public void SetShiireList(List<ShiireModel> results)
        {
            _currentShiireList = results ?? new List<ShiireModel>();
        }

        /// <summary>
        /// 一時的に保存している検索結果を取得する
        /// </summary>
        /// <returns>最新の検索結果</returns>
        public List<ShiireModel> GetShiireList()
        {
            return _currentShiireList;
        }

        public string? DetermineNavigationUri(string dateKeyword, string codeKeyword, List<ShiireModel> results, string actionType)
        {
            // データ無し
            if (results == null || results.Count == 0)
            {
                return null;
            }

            // 結果が複数
            if (results.Count > 1)
            {
                return $"/shiire/list?action={actionType}";
            }
            else
            {
                var target = results.First();

                if (actionType == "Edit")
                {
                    return $"/shiire/delete?date={target.ShiireNengappi}&code={target.ShohinCode}";
                }

                else if (actionType == "Delete")
                {
                    return $"/shiire/delete?date={target.ShiireNengappi}&code={target.ShohinCode}";
                }
                else
                {
                    return $"/shiire/list?action={actionType}";
                }
            }
        }


    }



}

