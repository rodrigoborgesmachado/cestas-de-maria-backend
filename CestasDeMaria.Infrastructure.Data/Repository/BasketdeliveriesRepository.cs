using Main = CestasDeMaria.Domain.Entities.Basketdeliveries;
using CestasDeMaria.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IBasketdeliveriesRepository;
using CestasDeMaria.Domain.Entities;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;

namespace CestasDeMaria.Infrastructure.Data.Repository
{
    public class BasketdeliveriesRepository : RepositoryBase<Main>, IMainRepository
    {
        public BasketdeliveriesRepository(CestasDeMariaContext currentContext)
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

        public async Task<IEnumerable<Main>> GetByFamilyAsync(long familyCode, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Familyid.Equals(familyCode));

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Main>> GetByWeekAndYearNumberAsync(int week, int weekReference, int year, bool onlyValid, string[] include = null)
        {
            var query = GetQueryable().Where(p => p.Weekofmonth.Equals(week) && p.Created.Year.Equals(year) && p.Families.DeliveryWeek.Equals(weekReference)).AsNoTracking();
            if (onlyValid)
                query = query.Where(p => p.Families.Familystatusid != 1 || (p.Families.Familystatusid == 1 && p.Deliverystatusid == 4));

            query = query.OrderBy(p => p.Families.Name);

            if (include != null)
            {
                foreach (var toInclude in include)
                {
                    query = query.Include(toInclude);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<Tuple<int, IEnumerable<Main>>> GetAllPagedAsync(int page, int quantity, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string[] include = null)
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
                query = query.Where(c => c.Families.Name.ToUpper().Contains(term.ToUpper()));
                
                if (!string.IsNullOrEmpty(Regex.Replace(term, @"\D", "")))
                {
                    query = query.Where(c => c.Families.Document.Contains(Regex.Replace(term, @"\D", "")));
                }
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
            var list = await GetAllPagedAsync(query, page, quantity, include, orderBy);

            return Tuple.Create(total, list);
        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var families = await _currentContext.Families
                .GroupBy(f => f.Familystatusid)
                .Select(g => new { StatusId = g.Key, Count = g.Count() })
                .ToListAsync();

            var deliveries = await _currentContext.Basketdeliveries
                .GroupBy(b => b.Deliverystatusid)
                .Select(g => new { StatusId = g.Key, Count = g.Count() })
                .ToListAsync();

            var quantityBaskterDelivered = _currentContext.Basketdeliveries.Where(d => d.Deliverystatusid.Equals(Enums.GetValue(DeliveryStatus.ENTREGUE))).Sum(b => b.Families.Basketquantity);
            var quantityBaskterNotDelivered = _currentContext.Basketdeliveries.Where(d => d.Deliverystatusid.Equals(Enums.GetValue(DeliveryStatus.FALTOU))).Sum(b => b.Families.Basketquantity);

            var deliveriesByWeekday = await _currentContext.Basketdeliveries
                .Where(b => b.Created >= startDate && b.Created <= endDate &&
                            b.Deliverystatusid.Equals(Enums.GetValue(DeliveryStatus.ENTREGUE)))
                .Join(_currentContext.Families,
                      b => b.Familyid,
                      f => f.Id,
                      (b, f) => new { b.Weekofmonth, f.Basketquantity }) // Get week and basket quantity
                .GroupBy(x => x.Weekofmonth)
                .Select(g => new
                {
                    Weekday = g.Key,
                    Count = g.Sum(x => x.Basketquantity) // Multiply by basket quantity
                })
                .ToListAsync();

            return new DashboardStatistics
            {
                QuantityFamilyInProgress = families.FirstOrDefault(f => f.StatusId == Enums.GetValue(FamilyStatus.EMATENDIMENTO))?.Count ?? 0,
                QuantityFamilyCutted = families.FirstOrDefault(f => f.StatusId == Enums.GetValue(FamilyStatus.CORTADO))?.Count ?? 0,
                QuantityFamilyEligible = families.FirstOrDefault(f => f.StatusId == Enums.GetValue(FamilyStatus.ELEGIVEL))?.Count ?? 0,
                QuantityFamilyWaiting = families.FirstOrDefault(f => f.StatusId == Enums.GetValue(FamilyStatus.EMESPERA))?.Count ?? 0,

                QuantityDeliveryPending = deliveries.FirstOrDefault(d => d.StatusId == Enums.GetValue(DeliveryStatus.SOLICITAR))?.Count ?? 0,
                QuantityDeliveryCompleted = deliveries.FirstOrDefault(d => d.StatusId == Enums.GetValue(DeliveryStatus.ENTREGUE))?.Count ?? 0,
                QuantityDeliveryMissed = deliveries.FirstOrDefault(d => d.StatusId == Enums.GetValue(DeliveryStatus.FALTOU))?.Count ?? 0,
                QuantityDeliveryCalled = deliveries.FirstOrDefault(d => d.StatusId == Enums.GetValue(DeliveryStatus.SOLICITADO))?.Count ?? 0,
                QuantityBasketDelivered = quantityBaskterDelivered,
                QuantityBasketNotDelivered = quantityBaskterNotDelivered,

                QuantityDeliveriesPerWeekday = deliveriesByWeekday.OrderBy(d => d.Weekday).ToDictionary(
                    d => d.Weekday.ToString(),
                    d => d.Count
                )
            };
        }
    }
}
