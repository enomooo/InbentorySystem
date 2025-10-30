using System.Data;

namespace InbentorySystem.Data
{
    // ★ データベース接続を作成する責務を持つインターフェース
    public interface IDbConnectionFactory
    {
        // ★ ここに書かれるべきメソッド
        IDbConnection CreateConnection();
    }
}