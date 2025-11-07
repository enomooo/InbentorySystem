using System.Data;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace InbentorySystem.Data
{

    public class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        /// <summary>
        /// 接続文字列を受け取る
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        public NpgsqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// NpgsqlConnectionを直接返すのではなく、抽象型で返す
        /// </summary>
        /// <returns>NpgsqlConnection(_connectionString)の新しいインスタンス</returns>
        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
