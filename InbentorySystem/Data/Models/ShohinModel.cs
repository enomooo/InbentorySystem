// Data/Models/ShohinModel.cs
namespace InbentorySystem.Data.Models
{
    /// <summary>
    /// PostagreSQLのM_SHOHINテーブルのデータを格納するモデルクラス
    /// </summary>
    public class ShohinModel
    {
        public string ShohinCode { get; set; } = string.Empty;

        public string ShohinKanjiMei { get; set; } = string.Empty;

        public string ShohinKanaMei { get; set; } = string.Empty;

        public int Siiirekakaku { get; set; }

        public int HanbaiKakaku { get; set; }
    }
}
