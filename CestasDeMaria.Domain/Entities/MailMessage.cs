namespace CestasDeMaria.Domain.Entities
{
    public class Mailmessage : BaseEntity
    {
        public long Adminid { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public string? To { get; set; }
        public string? Cc { get; set; }
        public int? Retries { get; set; }
        public string? Mailmessagefamilystatus { get; set; }
        public string? Message { get; set; }
    }
}
