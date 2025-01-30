namespace CestasDeMaria.Domain.Entities
{
    public class Familyfamilystatushistory : BaseEntity
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public long Familyid { get; set; }
        public long Oldfamilystatusid { get; set; }
        public long Newfamilystatusid { get; set; }
        public Families Families { get; set; }
        public Familystatus NewFamilystatus { get; set; }
        public Familystatus OldFamilystatus { get; set; }
    }
}
