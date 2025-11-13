using Dapper;
using InbentorySystem.Infrastructure.Models;
using InbentorySystem.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using Npgsql;

namespace InbentorySystem.Infrastructure.Repository
{
    public class ShiiresakiRepository : IShiiresakiRepository
    {
        private readonly string _connectionString;

        public ShiiresakiRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ShiiresakiModel>> GetAllAsync()
        {
            const string sql = @"
                SELECT
                    siiresaki_code AS ShiiresakiCode,
                    siiresaki_mei_kanji AS ShiiresakiMeiKanji,
                    siiresaki_mei_kana AS ShiiresakiMeiKana
                FROM
                    M_SHIIRESAKI
                ORDER BY
                    siiresaki_code;
                ";
            using (IDbConnection db = new NpgsqlConnection(_connectionString))
            {
                var result = await Dapper.SqlMapper.QueryAsync<ShiiresakiModel>(db, sql);
                return result.AsList();
            }
        }
    }
}
