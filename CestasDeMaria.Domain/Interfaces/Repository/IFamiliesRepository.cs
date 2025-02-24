using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using Main = CestasDeMaria.Domain.Entities.Families;

namespace CestasDeMaria.Domain.Interfaces.Repository
{
    public interface IFamiliesRepository : IRepositoryBase<Main>
    {
        Task<IEnumerable<Main>> GetAllAsync(string[] include = null);
        Task<IEnumerable<Main>> GetAllAsync(string parentCode, string[] include = null);
        Task<Main> GetAsync(long code, string[] include = null);
        Task<Main> GetByDocumentAsync(string document, string[] include = null);
        Task<Main> GetByPhoneAsync(string phone, string[] include = null);
        Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, Enums.FamilyStatus? status, string isActive = null, string term = null, string orderBy = null, string[] include = null);
        Task<IEnumerable<Main>> GetInProgressFamiliesAsync(int weekNumber, string[] include = null);
        Task<IEnumerable<Main>> GetEligibleFamiliesAsync(int weekNumber, string[] include = null);
        Task<IEnumerable<Main>> GetWaitingFamiliesAsync(string[] include = null);
    }
}
