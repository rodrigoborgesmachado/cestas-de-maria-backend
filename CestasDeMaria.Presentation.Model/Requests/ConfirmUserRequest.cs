namespace CestasDeMaria.Presentation.Model.Requests
{
    public class ConfirmUserRequest
    {
        public string Password { get; set; }
        public string VerifyPassword { get; set; }
        public string Guid { get; set; }
    }
}
