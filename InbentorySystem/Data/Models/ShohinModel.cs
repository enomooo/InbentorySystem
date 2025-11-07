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
        // 商品コード
        [Required(ErrorMessage = "商品コードは必須です。")]
        [StringLength(4, ErrorMessage = "商品コードは4文字以内です")]
        [DisplayName("商品コード")]
        public string ShohinCode { get; set; } = string.Empty;

        // 商品名（漢字）
        [Required(ErrorMessage = "商品名（漢字）は必須です。")]
        [StringLength(128, ErrorMessage = "商品名（漢字）は128文字以内です")]
        [DisplayName("商品名（漢字）")]
        public string ShohinMeiKanji { get; set; } = string.Empty;

        // 商品名（かな）
        [Required(ErrorMessage = "商品名（かな）は必須です。")]
        [StringLength(128, ErrorMessage = "商品名（かな）は128文字以内です")]
        [DisplayName("商品名（かな）")]
        public string ShohinMeiKana { get; set; } = string.Empty;

        // 仕入値
        [Required(ErrorMessage = "仕入値は必須です。")]
        [Range(0, 99999999, ErrorMessage = "価格は0円から99,999,999円の範囲で入力してください。")]
        [DisplayName("仕入値")]
        public int Shiirene { get; set; }

        // 売値
        [Required(ErrorMessage = "売値は必須です。")]
        [Range(0, 99999999, ErrorMessage = "価格は0円から99,999,999円の範囲で入力してください。")]
        [DisplayName("売値")]
        public int Urine { get; set; }

        // 仕入先コード
        [Required(ErrorMessage = "仕入先コードは必須です。")]
        [StringLength(4, ErrorMessage = "仕入先コードは4文字以内です")]
        [DisplayName("仕入先コード")]
        public string ShiiresakiCode { get; set; } = string.Empty;

        // Hack: DBのテーブルの定義と、ShohinModelクラスのフィールド定義を照合し、必要に応じてフィールドを追加・修正してください。
        [Range(-999, 999, ErrorMessage = "在庫数は-999個から999個の範囲で入力または計算される")]
        public int Suuryo { get; set; } = 0;
    }
}
