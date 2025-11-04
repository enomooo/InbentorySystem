namespace InbentorySystem.Data.Models
{
    // T_SHIIREテーブルに対応
    public class ShiireModel
    {
        public string ShiireNengappi { get; set; } = string.Empty;
        public string ShohinCode { get; set; } = string.Empty;

        public string ShiiresakiCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Shiirene { get; set; }
        public DateTime Tourokunichiji { get; set; }
    }
}
