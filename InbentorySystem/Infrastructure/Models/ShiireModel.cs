namespace InbentorySystem.Data.Models
{
    // T_SHIIREテーブルに対応
    public class ShiireModel
    {
        /// <summary>
        /// 仕入番号(自動採番)
        /// </summary>
        public string ShiireNo { get; set; } = string.Empty;

        /// <summary>
        /// 仕入年月日(画面のテキストボックスからのテキスト入力のためstring)
        /// DB上はDate型
        /// </summary>
        public DateTime? ShiireBi { get; set; }

        /// <summary>
        /// 商品コード
        /// </summary>
        public string ShohinCode { get; set; } = string.Empty;

        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string ShiiresakiCode { get; set; } = string.Empty;

        /// <summary>
        /// 数量（画面のテキストボックスの0表示を消すためint）
        /// </summary>
        public int? Quantity { get; set; }

        /// <summary>
        /// 仕入値
        /// </summary>
        public int Shiirene { get; set; }

        // todo: T_SHIIREには存在しないため、DBの規約に従い削除を検討
        public DateTime Tourokunichiji { get; set; }
    }
}
