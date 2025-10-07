using CestasDeMaria.Domain.Entities;
using Main = CestasDeMaria.Domain.Entities.Basketdeliveries;

namespace CestasDeMaria.Domain.Interfaces.Repository
{
    public interface IBasketdeliveriesRepository : IRepositoryBase<Main>
    {
        Task<IEnumerable<Main>> GetAllAsync(string[] include = null);
        Task<IEnumerable<Main>> GetAllAsync(string parentCode, string[] include = null);
        Task<Main> GetAsync(long code, string[] include = null);
        Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string[] include = null);
        Task<IEnumerable<Main>> GetByWeekAndYearNumberAsync(int week, int weekReference, int year, bool onlyValid, string[] include = null);
        Task<DashboardStatistics> GetDashboardStatisticsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Main>> GetByFamilyAsync(long familyCode, string[] include = null);

        Task DeleteByStatusAndDate(int week, int year, int status);
    }
}
