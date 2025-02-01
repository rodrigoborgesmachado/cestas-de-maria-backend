using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using MainDTO = CestasDeMaria.Application.DTO.FamiliesDTO;

namespace CestasDeMaria.Application.Interfaces
{
    public interface IFamiliesAppService : IDisposable
    {
        Task<IEnumerable<MainDTO>> GetAllAsync(string? include = null);

        Task<MainDTO> GetAsync(long code, string? include = null);

        Task<MainDTO> GetByDocumentAsync(string document, string? include = null);

        Task<MainDTO> InsertAsync(MainDTO mainDto);

        Task<MainDTO> UpdateAsync(long id, MainDTO mainDto);

        Task<MainDTO> RemoveAsync(MainDTO mainDto);

        Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, Enums.FamilyStatus? status, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<string> GetReport(int quantityMax, string isActive = null, string term = null, string orderBy = null, string? include = null);
    }
}
