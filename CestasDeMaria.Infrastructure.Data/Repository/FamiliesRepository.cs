using Main = CestasDeMaria.Domain.Entities.Families;
using CestasDeMaria.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IFamiliesRepository;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using CestasDeMaria.Infrastructure.CrossCutting.Mail;

namespace CestasDeMaria.Infrastructure.Data.Repository
{
    public class FamiliesRepository : RepositoryBase<Main>, IMainRepository
    {
        public FamiliesRepository(CestasDeMariaContext currentContext)
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

        public async Task<Main> GetByDocumentAsync(string document, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Document.Equals(document));

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.SingleOrDefaultAsync();
        }

        public async Task<Main> GetByPhoneAsync(string phone, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Phone.Equals(phone));

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Main>> GetEligibleFamiliesAsync(int weekNumber, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.DeliveryWeek.Equals(weekNumber) && p.Familystatusid.Equals(4)).OrderByDescending(c => c.Children).OrderByDescending(c => c.Adults).AsNoTracking();

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Main>> GetInProgressFamiliesAsync(int weekNumber, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.DeliveryWeek.Equals(weekNumber) && p.Familystatusid.Equals(3)).OrderByDescending(c => c.Children).OrderByDescending(c => c.Adults).AsNoTracking();

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Main>> GetWaitingFamiliesAsync(string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Familystatusid.Equals(2)).OrderByDescending(c => c.Children).OrderByDescending(c => c.Adults).AsNoTracking();

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, Enums.FamilyStatus? status, string isActive = null, string term = null, string orderBy = null, string[] include = null)
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
                var temp = Regex.Replace(term, @"\D", "");
                if(!string.IsNullOrEmpty(temp))
                {
                    query = query.Where(c => c.Document.Contains(temp) || c.Phone.Contains(temp));
                }
                else
                {
                    query = query.Where(c => c.Name.ToUpper().Contains(term.ToUpper()));
                }
            }

            if(status != null)
            {
                query = query.Where(f => f.Familystatusid.Equals(Enums.GetValue(status)));
            }

            var total = await GetAllPagedTotalAsync(query, include);
            var list = await GetAllPagedAsync(query, page, quantity, include, orderBy);

            return Tuple.Create(total, list);
        }
    }
}
