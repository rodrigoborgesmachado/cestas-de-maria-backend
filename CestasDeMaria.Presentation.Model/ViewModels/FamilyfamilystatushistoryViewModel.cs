namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class FamilyfamilystatushistoryViewModel : BaseViewModel
    {
        public long? Createdby { get; set; }
        public long? Updatedby { get; set; }
        public long Familyid { get; set; }
        public long Oldfamilystatusid { get; set; }
        public long Newfamilystatusid { get; set; }
        public FamiliesViewModel? Families { get; set; }
        public FamilystatusViewModel? NewFamilystatus { get; set; }
        public FamilystatusViewModel? OldFamilystatus { get; set; }
    }
}
