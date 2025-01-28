namespace CestasDeMaria.Domain.Entities
{
    public class Basketdeliveries : BaseEntity
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public long Familyid { get; set; }
        public long Deliverystatusid { get; set; }
        public int Weekofmonth { get; set; }
    }
}
