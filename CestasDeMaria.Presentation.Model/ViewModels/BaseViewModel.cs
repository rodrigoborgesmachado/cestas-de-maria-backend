namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class BaseViewModel
    {
        public long? Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public byte? IsActive { get; set; }
        public byte? IsDeleted { get; set; }
    }
}
