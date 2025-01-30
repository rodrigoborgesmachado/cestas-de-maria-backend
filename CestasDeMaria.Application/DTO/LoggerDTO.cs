namespace CestasDeMaria.Application.DTO
{
    public class LoggerDTO : BaseDTO
    {
        public string Message { get; set; }
        public long Adminid { get; set; }
        public string Classname { get; set; }
        public string Methodname { get; set; }
        public string Methodsignature { get; set; }
        public string Methodparameters { get; set; }
        public string Stacktrace { get; set; }
        public AdminsDTO Admins { get; set; }
    }
}
