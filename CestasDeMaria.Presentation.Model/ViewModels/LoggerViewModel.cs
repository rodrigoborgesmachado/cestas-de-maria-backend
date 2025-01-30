namespace CestasDeMaria.Presentation.Model.ViewModels
{
    public class LoggerViewModel : BaseViewModel
    {
        public string Message { get; set; }
        public long Adminid { get; set; }
        public string Classname { get; set; }
        public string Methodname { get; set; }
        public string Methodsignature { get; set; }
        public string Methodparameters { get; set; }
        public string Stacktrace { get; set; }
        public AdminsViewModel Admins { get; set; }
    }
}
