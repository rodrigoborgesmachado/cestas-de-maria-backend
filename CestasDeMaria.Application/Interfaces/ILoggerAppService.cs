﻿using Main = CestasDeMaria.Domain.Entities.Logger;
using MainDTO = CestasDeMaria.Application.DTO.LoggerDTO;

namespace CestasDeMaria.Application.Interfaces
{
    public interface ILoggerAppService : IDisposable
    {
        Task<Main> InsertAsync(string message, long userId);
        Task<Main> InsertAsync(string message);
        Task<Main> InsertAsync(Exception ex);
        Task<MainDTO> GetAsync(long code);
        Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllAsync(DateTime dateBegin, DateTime dateFinal, string filtro, int page = 0, int quantity = int.MaxValue, string orderby = "", string include = "");
        Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, string term = "", string orderby = "", string include = "");
    }
}
