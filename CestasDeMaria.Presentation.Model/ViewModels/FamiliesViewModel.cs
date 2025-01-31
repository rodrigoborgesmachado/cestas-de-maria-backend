namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class FamiliesViewModel : BaseViewModel
    {
        public long? Createdby { get; set; }
        public long? Updatedby { get; set; }
        public string Name { get; set; }
        public int Basketquantity { get; set; }
        public string Phone { get; set; }
        public string Document { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public byte? Issocialprogrambeneficiary { get; set; } = 0;
        public byte? Isfromlocal { get; set; } = 1;
        public string Housingsituation { get; set; }
        public byte? Hasseverelimitation { get; set; } = 0;
        public string Neighborhood { get; set; }
        public string Address { get; set; }
        public long? Familystatusid { get; set; }
        public int DeliveryWeek { get; set; }
        public FamilystatusViewModel? Familystatus { get; set; }
        public AdminsViewModel? Admins { get; set; }
        public IEnumerable<FamilyfamilystatushistoryViewModel>? Familyfamilystatushistory { get; set; }
        public IEnumerable<BasketdeliveriesViewModel>? Basketdeliveries { get; set; }
    }
}
