using System.Data;

namespace InbentorySystem.Data
{
    /// <summary>
    /// データベース接続を作成する責務を持つインターフェー
    /// </summary>
    public interface IDbConnectionFactory
    {
        // ★ ここに書かれるべきメソッド
        IDbConnection CreateConnection();
    }
}