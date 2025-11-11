using InbentorySystem.Data.Models;
using System.Collections.Generic;

namespace InbentorySystem.Services.Interfaces
{
    public interface IShiireService
    {
        List<ShiireModel> SearchResults { get; }
        string? LastDateFrom { get; }
        string? LastCodeKeyword { get; }

        ShiireModel? LastRegisteredShiire { get; }
        ShiireModel? LastEditedShiire { get; }
        ShiireModel? LastDeletedShiire { get; }

        string? DetermineNavigationUri(string dateFrom, string codeKeyword, List<ShiireModel> results, string actionType);
        void SetShiireList(List<ShiireModel> list);
        List<ShiireModel> GetShiireList();

        void SetLastRegisteredShiire(ShiireModel model);
        void SetLastEditedShiire(ShiireModel model);
        void SetLastDeletedShiire(ShiireModel model);

        void Clear();
    }
}