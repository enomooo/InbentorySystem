namespace InbentorySystem.Infrastructure.Models
{
    /// <summary>
    /// PostgreSQLのM\SHIIRESAKIテーブルのデータを格納するモデルクラス
    /// </summary>
    public class ShiiresakiModel
    {
        /// <summary>
        /// 仕入先コード(PK)
        /// </summary>
        public string ShiiresakiCode { get; set; }  = string.Empty;

        /// <summary>
        /// 仕入先名（漢字）
        /// </summary>
        public string ShiiresakiMeiKanji { get; set; } = string.Empty;

        /// <summary>
        /// 仕入先名（かな）
        /// </summary>
        public string ShiiresakiMeiKana { get; set; } = string.Empty;

    }

}