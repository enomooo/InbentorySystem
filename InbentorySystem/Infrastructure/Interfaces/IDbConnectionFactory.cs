using System.Data;

namespace InbentorySystem.Infrastructure.Interfaces
{
    /// <summary>
    /// データベース接続を作成する責務を持つインターフェイス
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}