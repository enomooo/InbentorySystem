// DataAnnotationsを使用するために追加
using System.ComponentModel.DataAnnotations;
// DisplayNameを使用するために追加
using System.ComponentModel;

namespace InbentorySystem.Data.Models
{
    /// <summary>
    /// PostagreSQLのM_SHOHINテーブルのデータを格納するモデルクラス
    /// DB側でintの桁数を制限出来なかったのでここで制限
    /// </summary>
    public class ShohinModel
    {
        /// <summary>
        /// 商品コード
        /// </summary>
        public string ShohinCode { get; set; } = string.Empty;

        /// <summary>
        /// 商品名（漢字）
        /// </summary>
        public string ShohinMeiKanji { get; set; } = string.Empty;

        /// <summary>
        /// 商品名（かな）
        /// </summary>
        public string ShohinMeiKana { get; set; } = string.Empty;

        /// <summary>
        /// 仕入値
        /// </summary>
        public int Shiirene { get; set; }

        /// <summary>
        /// 売値
        /// </summary>
        public int Urine { get; set; }

        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string ShiiresakiCode { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        public int Suuryo { get; set; } = 0;
    }
}
