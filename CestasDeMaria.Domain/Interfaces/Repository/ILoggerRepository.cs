using Main = CestasDeMaria.Domain.Entities.Logger;

namespace CestasDeMaria.Domain.Interfaces.Repository
{
    public interface ILoggerRepository : IRepositoryBase<Main>
    {
        Task<Main> InsertAsync(string message, long userId);
        Task<Main> GetAsync(long code);
        Task<Tuple<int, IEnumerable<Main>>> GetAllAsync(DateTime dateBegin, DateTime dateFinal, string filtro, int page, int quantity, string orderby, string[] include);
        Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, string term, string orderby, string[] include);
    }
}
