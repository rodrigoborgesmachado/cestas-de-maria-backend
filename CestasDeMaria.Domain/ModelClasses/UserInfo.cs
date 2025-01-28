namespace CestasDeMaria.Domain.ModelClasses
{
    public class UserInfo
    {
        public long id { get; set; }
        public string username { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
