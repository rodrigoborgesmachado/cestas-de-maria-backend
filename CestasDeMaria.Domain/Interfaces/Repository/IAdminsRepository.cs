using Main = CestasDeMaria.Domain.Entities.Admins;

namespace CestasDeMaria.Domain.Interfaces.Repository
{
    public interface IAdminsRepository : IRepositoryBase<Main>
    {
        Task<IEnumerable<Main>> GetAllAsync(string[] include = null);
        Task<IEnumerable<Main>> GetAllAsync(string parentCode, string[] include = null);
        Task<Main> GetAsync(long code, string[] include = null);
        Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, string isActive = null, string term = null, string orderBy = null, string[] include = null);
        Task<Main> GetByUsernameAsync(string username, string[] include = null);
    }
}
