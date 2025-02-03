using IBlobStorageService = CestasDeMaria.Domain.Interfaces.Services.IBlobStorageService;
using ILoggerService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IBasketdeliveriesRepository;
using IMainService = CestasDeMaria.Application.Interfaces.IBasketdeliveriesAppService;
using IFamiliesRepository = CestasDeMaria.Domain.Interfaces.Repository.IFamiliesRepository;
using IFamiliesHIstoryStatusService = CestasDeMaria.Application.Interfaces.IFamilyfamilystatushistoryAppService;
using Main = CestasDeMaria.Domain.Entities.Basketdeliveries;
using MainDTO = CestasDeMaria.Application.DTO.BasketdeliveriesDTO;
using Microsoft.Extensions.Options;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Application.Helpers;
using System.Globalization;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using CestasDeMaria.Application.DTO;

namespace CestasDeMaria.Application.Services
{
    public class BasketdeliveriesAppService : ServiceBase<MainDTO>, IMainService
    {
        private readonly IMainRepository _mainRepository;
        private readonly ILoggerService _loggerService;
        private readonly IFamiliesRepository _familiesRepository;
        private readonly IFamiliesHIstoryStatusService _familiesHIstoryStatusService;

        private string[] allowInclude = new string[] { "Families", "Families.Familystatus", "Families.Admins", "Basketdeliverystatus" };

        public BasketdeliveriesAppService(IBlobStorageService blobStorageService, IOptions<Settings> options, IMainRepository mainRepository, ILoggerService loggerService, IFamiliesRepository familiesService, IFamiliesHIstoryStatusService familiesHIstoryStatusRepository)
            : base(blobStorageService, options)
        {
            _mainRepository = mainRepository;
            _loggerService = loggerService;
            _familiesRepository = familiesService;
            _familiesHIstoryStatusService = familiesHIstoryStatusRepository;
        }

        public async Task<IEnumerable<MainDTO>> GetAllAsync(string? include = null)
        {
            var list = await _mainRepository.GetAllAsync(IncludesMethods.GetIncludes(include, allowInclude));
            return list.ProjectedAsCollection<MainDTO>();
        }

        public async Task<IEnumerable<MainDTO>> GetAllAsync(string parentCode, string? include = null)
        {
            var list = await _mainRepository.GetAllAsync(parentCode, IncludesMethods.GetIncludes(include, allowInclude));
            return list.ProjectedAsCollection<MainDTO>();
        }

        public async Task<MainDTO> GetAsync(long code, string? include = null)
        {
            var result = await _mainRepository.GetAsync(code, IncludesMethods.GetIncludes(include, allowInclude));
            return result.ProjectedAs<MainDTO>();
        }

        public async Task<Tuple<int, int, IEnumerable<MainDTO>>> GetAllPagedAsync(int page, int quantity, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            var tuple = await _mainRepository.GetAllPagedAsync(page, quantity, startDate, endDate, isActive, term, orderBy, IncludesMethods.GetIncludes(include, allowInclude));

            var total = tuple.Item1;
            var pages = (int)Math.Ceiling((double)total / quantity);

            var list = tuple.Item2.ProjectedAsCollection<MainDTO>();

            return Tuple.Create(total, pages, list);
        }

        public async Task<MainDTO> InsertAsync(MainDTO mainDto)
        {
            var main = mainDto.ProjectedAs<Main>();

            _mainRepository.Add(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> UpdateAsync(MainDTO mainDto)
        {
            var main = mainDto.ProjectedAs<Main>();
            main.Updated = DateTime.UtcNow;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> RemoveAsync(MainDTO mainDto)
        {
            var main = mainDto.ProjectedAs<Main>();

            _mainRepository.Remove(main);
            await _mainRepository.CommitAsync();

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<string> GetReport(int quantityMax, DateTime? startDate, DateTime? endDate, string isActive = null, string term = null, string orderBy = null, string? include = null)
        {
            await _loggerService.InsertAsync($"Report - Starting GetReport - {this.GetType().Name}");

            var result = await GetAllPagedAsync(1, quantityMax == 0 ? int.MaxValue : quantityMax, startDate, endDate, isActive, term, orderBy, include);
            string link = await UploadReport(result.Item3.ToList());

            await _loggerService.InsertAsync($"Report - Finishing GetReport - {this.GetType().Name}");
            return link;
        }

        public async Task<MainDTO> UpdateFamily(long id, long newFamilyId, long oldFamilyId, UserInfo user)
        {
            var newFamily = await _familiesRepository.GetAsync(newFamilyId);
            var oldFamily = await _familiesRepository.GetAsync(oldFamilyId);

            await _familiesHIstoryStatusService.InsertAsync(new DTO.FamilyfamilystatushistoryDTO()
            {
                Updatedby = user.id,
                Createdby = user.id,
                Familyid = oldFamily.Id,
                Newfamilystatusid = Enums.GetValue(FamilyStatus.EMESPERA),
                Oldfamilystatusid = oldFamily.Familystatusid,
            });

            oldFamily.Familystatusid = Enums.GetValue(FamilyStatus.EMESPERA);
            oldFamily.Updatedby = user.id;
            oldFamily.Updated = DateTime.Now;
            newFamily.DeliveryWeek = oldFamily.DeliveryWeek;
            newFamily.Familystatusid = Enums.GetValue(FamilyStatus.ELEGIVEL);

            _familiesRepository.Update(oldFamily);

            var main = await _mainRepository.GetAsync(id);
            if (main == null)
            {
                return null;
            }

            main.Familyid = newFamilyId;
            main.Families = newFamily;
            main.Updatedby = user.id;
            main.Updated = DateTime.Now;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            await _loggerService.InsertAsync($"Alterando atendimento da família {oldFamily.Name} para família {newFamily.Name} pelo usuário {user.username}", user.id);

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<MainDTO> UpdateStatus(long id, UserInfo user, DeliveryStatus status)
        {
            var main = await _mainRepository.GetAsync(id, IncludesMethods.GetIncludes("Families", allowInclude));
            if (main == null)
            {
                return null;
            }

            if(status == DeliveryStatus.SOLICITADO && main.Deliverystatusid != Enums.GetValue(DeliveryStatus.SOLICITAR) ||
               status == DeliveryStatus.SOLICITAR ||
               status == DeliveryStatus.ENTREGUE && main.Deliverystatusid != Enums.GetValue(DeliveryStatus.SOLICITADO) ||
               status == DeliveryStatus.FALTOU && main.Deliverystatusid != Enums.GetValue(DeliveryStatus.SOLICITADO))
            {
                throw new Exception("Não é possível mudar o status!");
            }

            if(status == DeliveryStatus.SOLICITADO && main.Families.Familystatusid.Equals(Enums.GetValue(FamilyStatus.ELEGIVEL)))
            {
                await _familiesHIstoryStatusService.InsertAsync(new DTO.FamilyfamilystatushistoryDTO() 
                { 
                    Updatedby = user.id,
                    Createdby = user.id,
                    Familyid = main.Familyid,
                    Newfamilystatusid = Enums.GetValue(FamilyStatus.EMATENDIMENTO),
                    Oldfamilystatusid = main.Families.Familystatusid,
                });

                main.Families.Familystatusid = Enums.GetValue(FamilyStatus.EMATENDIMENTO);
                main.Families.Updated = DateTime.Now;
                main.Families.Updatedby = user.id;
            }

            if (status == DeliveryStatus.FALTOU)
            {
                var history = await _familiesHIstoryStatusService.GetByFamilyAsync(main.Familyid);
                if (history != null && history.Count() >= 3)
                {
                    bool needToDelete = history
                        .OrderByDescending(p => p.Created) 
                        .Take(3) 
                        .All(p => p.Newfamilystatusid.Equals(Enums.GetValue(DeliveryStatus.FALTOU)));

                    if (needToDelete)
                    {
                        await _familiesHIstoryStatusService.InsertAsync(new DTO.FamilyfamilystatushistoryDTO()
                        {
                            Createdby = user.id,
                            Updatedby = user.id,
                            Familyid = main.Familyid,
                            Newfamilystatusid = Enums.GetValue(FamilyStatus.CORTADO),
                            Oldfamilystatusid = main.Families.Familystatusid
                        });

                        main.Families.Familystatusid = Enums.GetValue(FamilyStatus.CORTADO);

                        await _loggerService.InsertAsync($"Família {main.Families.Name} faltou às últimas 3 entregas, assim será cortada!", user.id);
                    }
                }
            }

            main.Deliverystatusid = Enums.GetValue(status);
            main.Updatedby = user.id;
            main.Updated = DateTime.Now;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            await _loggerService.InsertAsync($"Alterando status da entrega da família {main.Families.Name} para {Enums.GetDescription(status)} na semana {main.Weekofmonth} pelo usuário {user.username}", user.id);

            return main.ProjectedAs<MainDTO>();
        }

        public async Task<IEnumerable<MainDTO>> GetAndGenerateWeeklyBasketDeliveriesAsync(DateTime date, long user)
        {
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)date.DayOfWeek + 7) % 7;
            DateTime saturday = date.AddDays(daysUntilSaturday);
            int weekNumber = ISOWeek.GetWeekOfYear(saturday);
            int currentWeekNumber = ISOWeek.GetWeekOfYear(DateTime.Now);
            int weekReference = weekNumber % 4 == 0 ? 0 : 
                                weekNumber % 3 == 0 ? 3 :
                                weekNumber % 2 == 0 ? 2 : 1;

            var result = await _mainRepository.GetByWeekAndYearNumberAsync(weekNumber, saturday.Year, weekNumber >= currentWeekNumber, IncludesMethods.GetIncludes("Families.Familystatus,Basketdeliverystatus", allowInclude));

            if(result.Sum(r => r.Families.Basketquantity) >= 30 || weekNumber < currentWeekNumber)
            {
                return result.ProjectedAsCollection<MainDTO>();
            }
            

            List<Main> deliveries = new List<Main>();
            int remainingBaskets = 30 - result.Sum(r => r.Families.Basketquantity);

            // Get families in progress if there are any remainingBaskets
            var inProgressFamilies = await _familiesRepository.GetInProgressFamiliesAsync(weekReference);
            foreach (var family in inProgressFamilies)
            {
                if (remainingBaskets <= 0) break;

                deliveries.Add(new Main
                {
                    Familyid = family.Id,
                    Deliverystatusid = 1,
                    Weekofmonth = weekNumber,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    IsActive = 1,
                    IsDeleted = 0,
                    Createdby = user,
                    Updatedby = user
                });

                remainingBaskets -= family.Basketquantity;
            }

            // Get elegibles families if there are any remainingBaskets
            if (remainingBaskets > 0)
            {
                var eligibleFamilies = await _familiesRepository.GetEligibleFamiliesAsync(weekReference);
                foreach (var family in eligibleFamilies)
                {
                    if (remainingBaskets <= 0) break;

                    deliveries.Add(new Main
                    {
                        Familyid = family.Id,
                        Deliverystatusid = 1,
                        Weekofmonth = weekNumber,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        IsActive = 1,
                        IsDeleted = 0,
                        Createdby = user,
                        Updatedby = user
                    });

                    remainingBaskets -= family.Basketquantity;
                }
            }

            // Get families to put on elegible if there are any remainingBaskets
            if (remainingBaskets > 0)
            {
                var additionalFamilies = await _familiesRepository.GetWaitingFamiliesAsync();
                foreach (var family in additionalFamilies)
                {
                    if (remainingBaskets <= 0) break;

                    if (remainingBaskets - family.Basketquantity < 0) continue;
                    family.Familystatusid = 4;
                    family.DeliveryWeek = weekReference;

                    deliveries.Add(new Main
                    {
                        Familyid = family.Id,
                        Deliverystatusid = 1,
                        Weekofmonth = weekNumber,
                        Created = DateTime.UtcNow,
                        Updated = DateTime.UtcNow,
                        IsActive = 1,
                        IsDeleted = 0,
                        Createdby = user,
                        Updatedby = user
                    });

                    family.Updated = DateTime.UtcNow;
                    await _loggerService.InsertAsync($"Colocando família {family.Name} de Em Espera para Elegível para a semana {weekReference}", user);
                    await _familiesHIstoryStatusService.InsertAsync(new DTO.FamilyfamilystatushistoryDTO() 
                    { 
                        Createdby = user,
                        Updatedby = user,
                        Familyid = family.Id,
                        Newfamilystatusid = 4,
                        Oldfamilystatusid = 2,
                    });

                    _familiesRepository.Update(family);

                    remainingBaskets-= family.Basketquantity;
                }
            }

            if (deliveries.Count > 0)
            {
                _mainRepository.AddRange(deliveries);
                await _mainRepository.CommitAsync();
            }

            result = await _mainRepository.GetByWeekAndYearNumberAsync(weekNumber, saturday.Year, false, IncludesMethods.GetIncludes("Families.Familystatus,Basketdeliverystatus", allowInclude));

            return result.ProjectedAsCollection<MainDTO>();
        }

        public async Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var statistics = await _mainRepository.GetDashboardStatisticsAsync(startDate, endDate);
            return statistics.ProjectedAs<DashboardStatisticsDTO>();
        }

        public void Dispose()
        {
            _mainRepository.Dispose();
        }
    }
}
