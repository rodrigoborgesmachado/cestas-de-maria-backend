using Main = CestasDeMaria.Domain.Entities.Familyfamilystatushistory;
using CestasDeMaria.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IFamilyfamilystatushistoryRepository;

namespace CestasDeMaria.Infrastructure.Data.Repository
{
    public class FamilyfamilystatushistoryRepository : RepositoryBase<Main>, IMainRepository
    {
        public FamilyfamilystatushistoryRepository(CestasDeMariaContext currentContext)
            : base(currentContext)
        {
        }

        public async Task<IEnumerable<Main>> GetAllAsync(string[] include = null)
        {
            return await GetAllAsync(s => s.IsDeleted.Equals(0), include, orderBy: "Created: Desc");
        }

        public async Task<IEnumerable<Main>> GetAllAsync(string parentCode, string[] include = null)
        {
            return await GetAllAsync(s => s.IsDeleted.Equals(0), include, orderBy: "Created: Desc");
        }

        public async Task<Main> GetAsync(long code, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Id.Equals(code));

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Main>> GetByFamilyAsync(long code, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Familyid.Equals(code));

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, string isActive = null, string term = null, string orderBy = null, string[] include = null)
        {
            var query = GetQueryable();

            if (!string.IsNullOrEmpty(isActive))
            {
                Regex regexObj = new Regex(@"[^\d]");

                string isActiveString = regexObj.Replace(isActive, "");

                int isActiveInt32 = 0;

                if (!string.IsNullOrEmpty(isActiveString))
                {
                    isActiveInt32 = Convert.ToInt32(regexObj.Replace(isActive, ""));
                }

                if (isActiveInt32 > 1)
                {
                    isActiveInt32 = 1;
                }

                byte isActiveByte = Convert.ToByte(isActiveInt32);

                query = query.Where(c => c.IsActive.Equals(isActiveByte));
            }

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(c => c.Id.Equals(term));
            }

            var total = await GetAllPagedTotalAsync(query, include);
            var list = await GetAllPagedAsync(query, page, quantity, include, orderBy);

            return Tuple.Create(total, list);
        }
    }
}
