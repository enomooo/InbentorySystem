using System.Collections.Generic;
using System.Threading.Tasks;
using InbentorySystem.Infrastructure.Models;

namespace InbentorySystem.Infrastructure.Interfaces
{
    public interface IShiiresakiRepository
    {
        /// <summary>
        /// すべての仕入先マスタを取得する
        /// </summary>
        /// <returns>仕入先モデルのリスト</returns>
        Task<List<ShiiresakiModel>> GetAllAsync();
    }
}
