namespace InbentorySystem.Data.Models
{
    // T_SHIIREテーブルに対応
    public class ShiireModel
    {
        /// <summary>
        /// 仕入年月日
        /// </summary>
        public DateTime ShiireNengappi { get; set; }

        /// <summary>
        /// 商品コード
        /// </summary>
        public string ShohinCode { get; set; } = string.Empty;

        /// <summary>
        /// 仕入先コード
        /// </summary>
        public string ShiiresakiCode { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 仕入値
        /// </summary>
        public int Shiirene { get; set; }

        // todo: 登録日はない（shiirebi,uriagebiで統一する）
        public DateTime Tourokunichiji { get; set; }

        /// <summary>
        /// 仕入番号
        /// </summary>
        public string ShiireNo { get; set; } = string.Empty;
    }
}
