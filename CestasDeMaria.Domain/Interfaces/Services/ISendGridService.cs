using Main = CestasDeMaria.Domain.Entities.Mailmessage;

namespace CestasDeMaria.Domain.Interfaces.Services
{
    public interface ISendGridService
    {
        Task<bool> SendMail(Main entity);
    }
}
