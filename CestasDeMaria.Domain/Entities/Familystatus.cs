namespace CestasDeMaria.Domain.Entities
{
    public class Familystatus : BaseEntity
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public string Description { get; set; }
    }
}
