namespace CestasDeMaria.Domain.ModelClasses
{
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string username { get; set; }
        public string id { get; set; }
        public string issued { get; set; }
        public string expires { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string isadmin { get; set; }
    }
}
