using CestasDeMaria.Application.DTO;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Infrastructure.CrossCutting.Mail;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;
using MainDTO = CestasDeMaria.Application.DTO.BasketdeliveriesDTO;

namespace CestasDeMaria.Application.Interfaces
{
    public interface IBasketdeliveriesAppService : IDisposable
    {
        Task<IEnumerable<MainDTO>> GetAllAsync(string? include = null);

        Task<MainDTO> GetAsync(long code, string? include = null);

        Task<MainDTO> InsertAsync(MainDTO mainDto);

        Task<MainDTO> UpdateAsync(MainDTO mainDto);

        Task<MainDTO> RemoveAsync(MainDTO mainDto);

        Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<string> GetReport(int quantityMax, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<IEnumerable<MainDTO>> GetAndGenerateWeeklyBasketDeliveriesAsync(DateTime date, long user);

        Task<MainDTO> UpdateStatus(long id, UserInfo user, DeliveryStatus status);

        Task<MainDTO> UpdateFamily(long id, long newFamilyId, long oldFamilyId, UserInfo user);

        Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(DateTime startDate, DateTime endDate);

        Task<string> GetFullReport();

        Task<IEnumerable<MainDTO>> GetByFamilyAsync(long familyCode, string? include = null);
    }
}
