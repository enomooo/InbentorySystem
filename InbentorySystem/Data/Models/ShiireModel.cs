namespace InbentorySystem.Data.Models
{
    // T_SHIIREテーブルに対応
    public class ShiireModel
    {
        public DateTime ShiireNengappi { get; set; }
        public string ShohinCode { get; set; } = string.Empty;

        public string ShiiresakiCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Shiirene { get; set; }
        public DateTime Tourokunichiji { get; set; }
        public string ShiireNo { get; set; } = string.Empty;
    }
}
