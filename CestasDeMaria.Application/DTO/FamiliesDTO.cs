namespace CestasDeMaria.Application.DTO
{
    public class FamiliesDTO : BaseDTO
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
        public FamilystatusDTO Familystatus { get; set; }
        public AdminsDTO Admins { get; set; }
        public IEnumerable<FamilyfamilystatushistoryDTO> Familyfamilystatushistory { get; set; }
    }
}
