// Data/Models/ShohinModel.cs
namespace InbentorySystem.Data.Models
{
    /// <summary>
    /// PostagreSQLのM_SHOHINテーブルのデータを格納するモデルクラス
    /// </summary>
    public class ShohinModel
    {
        // M_SHOHINテーブルのカラムに対応するフィールド
        public string ShohinCode { get; set; } = string.Empty;
        public string ShohinMeiKanji { get; set; } = string.Empty;
        public string ShohinMeiKana { get; set; } = string.Empty;
        public decimal ShiireKakaku { get; set; }
        public decimal HanbaiKakaku { get; set; }
        public string? ShiiresakiCode { get; set; }

        // Hack: DBのテーブルの定義と、ShohinModelクラスのフィールド定義を照合し、必要に応じてフィールドを追加・修正してください。
        public int Suuryo { get; set; } = 0;
    }
}
