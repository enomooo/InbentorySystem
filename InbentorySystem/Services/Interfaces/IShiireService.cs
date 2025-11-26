using InbentorySystem.Data.Models;
using System.Collections.Generic;

namespace InbentorySystem.Services.Interfaces
{
    public interface IShiireService
    {
        List<ShiireModel> SearchResults { get; }
        string? LastCodeKeyword { get; }

        ShiireModel? LastRegisteredShiire { get; }
        ShiireModel? LastEditedShiire { get; }
        ShiireModel? LastDeletedShiire { get; }

        public int? LastEditBeforeZaikoQuantity { get; }
        public int? LastEditAfterZaikoQuantity { get; }

        void SetLastEditResults(ShiireModel original, ShiireModel updated, int beforeZaiko, int afterZaiko);
        string? DetermineNavigationUri(string dateFrom, string codeKeyword, List<ShiireModel> results, string actionType);
        void SetShiireList(List<ShiireModel> list);
        List<ShiireModel> GetShiireList();

        void SetSearchResults(List<ShiireModel> results);
        List<ShiireModel> GetSearchResults();

        void SetLastRegisteredShiire(ShiireModel model);
        ShiireModel? GetLastRegisteredShiire();

        void SetLastEditedShiire(ShiireModel model);
        ShiireModel? GetLastEditedShiire();

        void SetLastEditResult(ShiireModel model);
        ShiireModel? GetLastEditResult();

        void SetLastDeleteResult(ShiireModel model);
        ShiireModel? GetLastDeleteResult();
        public void SetLastDeletedShiire(ShiireModel model);

        void Clear();
    }
}