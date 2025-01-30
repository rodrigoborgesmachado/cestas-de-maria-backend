using MainDTO = CestasDeMaria.Application.DTO.AdminsDTO;

namespace CestasDeMaria.Application.Interfaces
{
    public interface IAdminsAppService : IDisposable
    {
        Task<IEnumerable<MainDTO>> GetAllAsync(string? include = null);

        Task<MainDTO> GetByUsernameAsync(string username, string? include = null);

        Task<MainDTO> GetAsync(long code, string? include = null);

        Task<MainDTO> InsertAsync(MainDTO mainDto);

        Task<MainDTO> UpdateAsync(MainDTO mainDto);

        Task<MainDTO> RemoveAsync(MainDTO mainDto);

        Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<string> GetReport(int quantityMax, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<MainDTO> InactiveUserAsync(long id, long user);

        Task<MainDTO> ActiveUserAsync(long id, long user);
    }
}
