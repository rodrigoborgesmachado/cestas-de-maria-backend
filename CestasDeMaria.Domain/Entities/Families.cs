namespace CestasDeMaria.Domain.Entities
{
    public class Families : BaseEntity
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public string Name { get; set; }
        public int Basketquantity { get; set; }
        public string Phone { get; set; }
        public string Document { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public byte Issocialprogrambeneficiary { get; set; }
        public byte Isfromlocal { get; set; }
        public string Housingsituation { get; set; }
        public byte Hasseverelimitation { get; set; }
        public string Neighborhood { get; set; }
        public string Address { get; set; }
        public long Familystatusid { get; set; }
        public int DeliveryWeek { get; set; }
        public Familystatus Familystatus { get; set; }
        public Admins Admins { get; set; }
        public IEnumerable<Familyfamilystatushistory> Familyfamilystatushistory { get; set; }
    }
}
