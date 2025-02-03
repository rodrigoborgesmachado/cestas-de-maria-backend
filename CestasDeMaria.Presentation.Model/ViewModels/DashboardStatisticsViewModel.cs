namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class DashboardStatisticsViewModel
    {
        public int QuantityFamilyInProgress { get; set; }
        public int QuantityFamilyCutted { get; set; }
        public int QuantityFamilyEligible { get; set; }
        public int QuantityFamilyWaiting { get; set; }

        public int QuantityDeliveryPending { get; set; }
        public int QuantityDeliveryCompleted { get; set; }
        public int QuantityDeliveryMissed { get; set; }
        public int QuantityDeliveryCalled { get; set; }
        public Dictionary<string, int> QuantityDeliveriesPerWeekday { get; set; } = new();
    }
}
