namespace CestasDeMaria.Application.DTO
{
    public class FamilyfamilystatushistoryDTO : BaseDTO
    {
        public long Createdby { get; set; }
        public long Updatedby { get; set; }
        public long Familyid { get; set; }
        public long Oldfamilystatusid { get; set; }
        public long Newfamilystatusid { get; set; }
    }
}
