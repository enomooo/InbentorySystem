// DataAnnotationsを使用するために追加
using System.ComponentModel.DataAnnotations;
// DisplayNameを使用するために追加
using System.ComponentModel;

namespace InbentorySystem.Data.Models
{
    /// <summary>
    /// PostagreSQLのM_SHOHINテーブルのデータを格納するモデルクラス
    /// </summary>
    public class ShohinModel
    {
        // todo:  DB側でintの桁数を制限出来なかったのでここで制限

        /// <summary>
        /// 商品コード
        /// </summary>
        [Required(ErrorMessage = "商品コードは必須入力です")]
        public string ShohinCode { get; set; } = string.Empty;

        /// <summary>
        /// 商品名（漢字）
        /// </summary>
        [Required(ErrorMessage = "商品名（漢字）は必須入力です")]
        public string ShohinMeiKanji { get; set; } = string.Empty;

        /// <summary>
        /// 商品名（かな）
        /// </summary>
        [Required(ErrorMessage = "商品名（かな）は必須入力です")]
        public string ShohinMeiKana { get; set; } = string.Empty;

        /// <summary>
        /// 仕入値
        /// </summary>
        [Required(ErrorMessage = "仕入値は必須入力です")]
        public int Shiirene { get; set; }

        /// <summary>
        /// 売値
        /// </summary>
        [Required(ErrorMessage = "売値は必須入力です")]
        public int Urine { get; set; }

        /// <summary>
        /// 仕入先コード
        /// </summary>
        [Required(ErrorMessage = "仕入先コードは必須選択です")]
        public string ShiiresakiCode { get; set; } = string.Empty;
    }
}
