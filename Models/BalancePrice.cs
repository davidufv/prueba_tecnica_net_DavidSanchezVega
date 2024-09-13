namespace prueba_tecnica_net.Models
{

    public class BalancePrice
    {
        public decimal DownRegPrice { get; set; }
        public decimal DownRegPriceFrrA { get; set; }
        public decimal ImblPurchasePrice { get; set; }
        public decimal ImblSalesPrice { get; set; }
        public decimal ImblSpotDifferencePrice { get; set; }
        public decimal IncentivisingComponent { get; set; }
        public decimal MainDirRegPowerPerMBA { get; set; }
        public string Mba { get; set; }
        public string Timestamp { get; set; }
        public string TimestampUTC { get; set; }
        public decimal UpRegPrice { get; set; }
        public decimal UpRegPriceFrrA { get; set; }
        public decimal ValueOfAvoidedActivation { get; set; }
    }

}
