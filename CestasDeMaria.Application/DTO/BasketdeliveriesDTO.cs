namespace CestasDeMaria.Application.DTO
{
    public class BasketdeliveriesDTO : BaseDTO
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public long Familyid { get; set; }
        public long Deliverystatusid { get; set; }
        public int Weekofmonth { get; set; }
    }
}
