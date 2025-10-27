// Data/Models/ShohinModel.cs
namespace InbentorySystem.Data.Models
{
    /// <summary>
    /// PostagreSQLのM_SHOHINテーブルのデータを格納するモデルクラス
    /// </summary>
    public class ShohinModel
    {
        // M_SHOHINテーブルのカラムに対応するフィールド
        public string ShohinCode { get; set; }
        public string ShohinMeiKanji { get; set; }
        public string ShohinMeiKana { get; set; }
        public decimal ShiireKakaku { get; set; }
        public decimal HanbaiKakaku { get; set; }
        public string? ShiiresakiCode { get; set; }

        // Hack: DBのテーブルの定義と、ShohinModelクラスのフィールド定義を照合し、必要に応じてフィールドを追加・修正してください。
        public int Suuryo { get; set; } = 0;
    }
}
