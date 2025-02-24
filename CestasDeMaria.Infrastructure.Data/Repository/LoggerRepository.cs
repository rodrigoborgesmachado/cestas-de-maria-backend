using CestasDeMaria.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.ILoggerRepository;
using Main = CestasDeMaria.Domain.Entities.Logs;

namespace CestasDeMaria.Infrastructure.Data.Repository
{
    public class LoggerRepository : RepositoryBase<Main>, IMainRepository
    {
        public LoggerRepository(CestasDeMariaContext currentContext)
            : base(currentContext)
        {
        }

        public async Task<Main> InsertAsync(string message, long userId)
        {
            var main = new Main(message, userId);

            Add(main);
            await CommitAsync();

            return main;
        }

        public async Task<Main> GetAsync(long code)
        {
            try
            {
                var query = GetQueryable().Where(p => p.Id.Equals(code));

                return await query.SingleOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tuple<int, IEnumerable<Main>>> GetAllAsync(DateTime dateBegin, DateTime dateFinal, string filtro, int page, int quantity, string orderby, string[] include)
        {
            try
            {
                var query = GetQueryable();

                query = query.Where(p => p.Created <= dateFinal && p.Created >= dateBegin);

                if (!string.IsNullOrEmpty(filtro))
                {
                    query = query.Where(p => p.Message.Contains(filtro));
                }

                var total = await GetAllPagedTotalAsync(query, include);
                var list = await GetAllPagedAsync(query, page, quantity, include, orderby);

                return Tuple.Create(total, list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, DateTime? startDate, DateTime? endDate, string term, string orderby, string[] include)
        {
            try
            {
                var query = GetQueryable();

                if (!string.IsNullOrEmpty(term))
                {
                    query = query.Where(p => p.Message.Contains(term));
                }

                if (startDate != null)
                {
                    query = query.Where(o => o.Created >= startDate);
                }
                if (endDate != null)
                {
                    query = query.Where(o => o.Created <= endDate);
                }

                var total = await GetAllPagedTotalAsync(query, include);
                var list = await GetAllPagedAsync(query, page, quantity, include, orderby);

                return Tuple.Create(total, list);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
