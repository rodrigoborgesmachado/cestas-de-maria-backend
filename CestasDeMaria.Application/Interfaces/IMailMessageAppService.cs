﻿using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using MainDTO = CestasDeMaria.Application.DTO.MailmessageDTO;

namespace CestasDeMaria.Application.Interfaces
{
    public interface IMailMessageAppService : IDisposable
    {
        Task<IEnumerable<MainDTO>> GetAllAsync(string? include = null);

        Task<MainDTO> GetAsync(long code, string? include = null);

        Task<MainDTO> InsertAsync(MainDTO mainDto);

        Task<MainDTO> UpdateAsync(MainDTO mainDto);

        Task<MainDTO> RemoveAsync(MainDTO mainDto);

        Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<string> GetReport(int quantityMax, string isActive = null, string term = null, string orderBy = null, string? include = null);

        Task<MainDTO> SendMail(string to, string[] values, Enums.EmailType typeMail);

        Task<MainDTO> SendMail(string mail, string to, string subject);

        Task<MainDTO> ResendMail(long code);
    }
}
