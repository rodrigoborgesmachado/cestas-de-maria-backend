namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class BasketdeliveriesViewModel : BaseViewModel
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public long Familyid { get; set; }
        public long Deliverystatusid { get; set; }
        public int Weekofmonth { get; set; }
    }
}
