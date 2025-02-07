namespace CestasDeMaria.Application.DTO
{
    public class DashboardStatisticsDTO
    {
        public int QuantityFamilyInProgress { get; set; }
        public int QuantityFamilyCutted { get; set; }
        public int QuantityFamilyEligible { get; set; }
        public int QuantityFamilyWaiting { get; set; }

        public int QuantityDeliveryPending { get; set; }
        public int QuantityDeliveryCompleted { get; set; }
        public int QuantityDeliveryMissed { get; set; }
        public int QuantityDeliveryCalled { get; set; }
        public int QuantityBasketDelivered { get; set; }
        public int QuantityBasketNotDelivered { get; set; }
        public Dictionary<string, int> QuantityDeliveriesPerWeekday { get; set; } = new();
    }
}
