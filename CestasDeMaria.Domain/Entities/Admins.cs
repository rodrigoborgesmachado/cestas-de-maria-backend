namespace CestasDeMaria.Domain.Entities
{
    public class Admins : BaseEntity
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Passwordhash { get; set; }
    }
}
