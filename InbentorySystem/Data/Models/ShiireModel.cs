namespace InbentorySystem.Data.Models
{
    // T_SHIIREテーブルに対応
    public class ShiireModel
    {
        public string ShiireNengappi { get; set; }
        public string ShohinCode { get; set; }

        public string ShiiresakiCode { get; set; }
        public int Quantity { get; set; }
        public decimal Shiirene { get; set; }
        public DateTime Tourokunichiji { get; set; }
    }
}
