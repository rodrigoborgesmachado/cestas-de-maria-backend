using CestasDeMaria.Application.DTO;
using CestasDeMaria.Application.Helpers;
using CestasDeMaria.Domain.Entities;
using CestasDeMaria.Domain.ModelClasses;
using CestasDeMaria.Infrastructure.CrossCutting.Enums;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using static CestasDeMaria.Infrastructure.CrossCutting.Enums.Enums;
using IBlobStorageService = CestasDeMaria.Domain.Interfaces.Services.IBlobStorageService;
using IFamiliesHIstoryStatusService = CestasDeMaria.Application.Interfaces.IFamilyfamilystatushistoryAppService;
using IFamiliesRepository = CestasDeMaria.Domain.Interfaces.Repository.IFamiliesRepository;
using ILoggerService = CestasDeMaria.Application.Interfaces.ILoggerAppService;
using IMainRepository = CestasDeMaria.Domain.Interfaces.Repository.IBasketdeliveriesRepository;
using IMainService = CestasDeMaria.Application.Interfaces.IBasketdeliveriesAppService;
using Main = CestasDeMaria.Domain.Entities.Basketdeliveries;
using MainDTO = CestasDeMaria.Application.DTO.BasketdeliveriesDTO;

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
            main.Deliverystatusid = (int)DeliveryStatus.SOLICITAR;

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

            main.Deliverystatusid = Enums.GetValue(status);
            main.Updatedby = user.id;
            main.Updated = DateTime.Now;

            _mainRepository.Update(main);
            await _mainRepository.CommitAsync();

            var result = await _mainRepository.GetByWeekAndYearNumberAsync(weekNumber, 0, saturday.Year, (weekNumber > currentWeekNumber || !(weekNumber == currentWeekNumber && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)), IncludesMethods.GetIncludes("Families.Familystatus,Basketdeliverystatus", allowInclude));
            var invalidDeliveries = result.Where(r => !r.Families.DeliveryWeek.Equals(weekReference)).ToList();
            if (invalidDeliveries.Any() && weekNumber >= currentWeekNumber)
            {
                _mainRepository.RemoveRange(invalidDeliveries);
                await _mainRepository.CommitAsync();
                result = result.Except(invalidDeliveries).ToList();
            }

            result = result.Where(r => r.Families.DeliveryWeek.Equals(weekReference))
                .GroupBy(i => i.Families.Id)
                .Select(g => g.First())
                .ToList();

            if (weekNumber < currentWeekNumber)
            {
                return result.ProjectedAsCollection<MainDTO>();
            }
            if (result.Sum(r => r.Families.Basketquantity) >= 30)
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)date.DayOfWeek + 7) % 7;
            DateTime saturday = date.AddDays(daysUntilSaturday);
            int weekNumber = ISOWeek.GetWeekOfYear(saturday);
            int currentWeekNumber = ISOWeek.GetWeekOfYear(DateTime.Now);
            int weekReference = (weekNumber % 4 == 0) ? 4 : weekNumber % 4;

            var result = await _mainRepository.GetByWeekAndYearNumberAsync(weekNumber, weekReference, saturday.Year, (weekNumber > currentWeekNumber || !(weekNumber == currentWeekNumber && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)), IncludesMethods.GetIncludes("Families.Familystatus,Basketdeliverystatus", allowInclude));
            result = result.Where(r => r.Families.DeliveryWeek.Equals(weekReference));

            result = result
                    .GroupBy(i => i.Families.Id) 
                    .Select(g => g.First()) 
                    .ToList(); 

            if (result.Sum(r => r.Families.Basketquantity) >= 30 || weekNumber < currentWeekNumber)
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
                if (result.Any(i => i.Families.Id.Equals(family.Id))) continue;
                if (deliveries.Exists(d => d.Familyid.Equals(family.Id))) continue;

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
                    if (result.Any(i => i.Families.Id.Equals(family.Id))) continue;
                    if (deliveries.Exists(d => d.Familyid.Equals(family.Id))) continue;

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

            if (remainingBaskets > 0)
            {
                var additionalFamilies = await _familiesRepository.GetWaitingFamiliesAsync();
                foreach (var family in additionalFamilies)
                {
                    if (remainingBaskets <= 0) break;
                    if (result.Any(i => i.Families.Id.Equals(family.Id))) continue;
                    if (deliveries.Exists(d => d.Familyid.Equals(family.Id))) continue;
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
            result = await _mainRepository.GetByWeekAndYearNumberAsync(weekNumber, 0, saturday.Year, (weekNumber > currentWeekNumber || !(weekNumber == currentWeekNumber && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)), IncludesMethods.GetIncludes("Families.Familystatus,Basketdeliverystatus", allowInclude));
            result = result.Where(r => r.Families.DeliveryWeek.Equals(weekReference))
                .GroupBy(i => i.Families.Id)
                .Select(g => g.First())
                .ToList();
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

            result = await _mainRepository.GetByWeekAndYearNumberAsync(weekNumber, weekReference, saturday.Year, (weekNumber > currentWeekNumber || !(weekNumber == currentWeekNumber && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)), IncludesMethods.GetIncludes("Families.Familystatus,Basketdeliverystatus", allowInclude));
            result = result.Where(r => r.Families.DeliveryWeek.Equals(weekReference));

            return result.ProjectedAsCollection<MainDTO>();
        }

        public async Task<DashboardStatisticsDTO> GetDashboardStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            var statistics = await _mainRepository.GetDashboardStatisticsAsync(startDate, endDate);
            return statistics.ProjectedAs<DashboardStatisticsDTO>();
        }

        public async Task<string> GetFullReport()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            await _loggerService.InsertAsync($"Report - Starting GetReport - {this.GetType().Name}");

            var families = await _familiesRepository.GetAllAsync(new string[] { "Familystatus" });
            var deliveries = await _mainRepository.GetAllAsync(IncludesMethods.GetIncludes("Basketdeliverystatus", allowInclude));

            int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            // Generate Excel file
            byte[] bytes = GenerateExcelReport(families, deliveries);

            string fileName = $"FullReport${unixTimestamp}.xlsx";
            var link = await _iBlobStorageService.UploadFileAsync(bytes, fileName);

            stopWatch.Stop();
            await _loggerService.InsertAsync($"Report - Finishing GetReport - {this.GetType().Name} in {ElapsedTime(stopWatch)}");

            return link;
        }

        private string ElapsedTime(Stopwatch stopWatch)
        {
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
        }

        private byte[] GenerateExcelReport(IEnumerable<Families> families, IEnumerable<Basketdeliveries> deliveries)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Families Report");

                worksheet.Cells[1, 1].Value = "Nome da Família";
                worksheet.Cells[1, 2].Value = "Telefone";
                worksheet.Cells[1, 3].Value = "Documento";
                worksheet.Cells[1, 4].Value = "Adultos";
                worksheet.Cells[1, 5].Value = "Crianças";
                worksheet.Cells[1, 6].Value = "Quantidade de Cestas";
                worksheet.Cells[1, 7].Value = "Situação da Moradia";
                worksheet.Cells[1, 8].Value = "Bairro";
                worksheet.Cells[1, 9].Value = "Endereço";
                worksheet.Cells[1, 10].Value = "Status da Família";
                worksheet.Cells[1, 11].Value = "Último Status de Entrega";
                worksheet.Cells[1, 12].Value = "Última Data de Entrega";

                var saturdays = deliveries
                                .Select(d => GetSaturdayOfWeek(d.Created.Year, d.Weekofmonth))
                                .Distinct()
                                .OrderBy(d => d)
                                .ToList();


                int columnIndex = 13;
                var saturdayColumns = new Dictionary<(int Year, int WeekOfYear), int>();

                foreach (var saturday in saturdays)
                {
                    int year = saturday.Year;
                    int weekOfYear = ISOWeek.GetWeekOfYear(saturday);

                    worksheet.Cells[1, columnIndex].Value = saturday.ToString("yyyy-MM-dd");
                    worksheet.Cells[1, columnIndex].Style.Font.Bold = true;
                    worksheet.Column(columnIndex).AutoFit();

                    saturdayColumns[(year, weekOfYear)] = columnIndex;
                    columnIndex++;
                }

                using (var headerRange = worksheet.Cells[1, 1, 1, 12])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.AutoFitColumns();
                }

                int row = 2;
                foreach (var family in families)
                {
                    var lastDelivery = deliveries
                        .Where(d => d.Familyid == family.Id)
                        .OrderByDescending(d => d.Created)
                        .FirstOrDefault();

                    worksheet.Cells[row, 1].Value = family.Name;
                    worksheet.Cells[row, 2].Value = family.Phone;
                    worksheet.Cells[row, 3].Value = family.Document;
                    worksheet.Cells[row, 4].Value = family.Adults;
                    worksheet.Cells[row, 5].Value = family.Children;
                    worksheet.Cells[row, 6].Value = family.Basketquantity;
                    worksheet.Cells[row, 7].Value = family.Housingsituation.ToUpper();
                    worksheet.Cells[row, 8].Value = family.Neighborhood;
                    worksheet.Cells[row, 9].Value = family.Address;
                    worksheet.Cells[row, 10].Value = family.Familystatus?.Description ?? "Unknown";
                    worksheet.Cells[row, 11].Value = lastDelivery?.Basketdeliverystatus?.Description ?? "No Deliveries";
                    worksheet.Cells[row, 12].Value = lastDelivery?.Created.ToString("yyyy-MM-dd") ?? "N/A";

                    var familyDeliveries = deliveries.Where(d => d.Familyid == family.Id);
                    foreach (var delivery in familyDeliveries)
                    {
                        int deliveryYear = delivery.Created.Year;
                        int deliveryWeek = delivery.Weekofmonth;

                        if (saturdayColumns.TryGetValue((deliveryYear, deliveryWeek), out int col))
                        {
                            var status = delivery.Basketdeliverystatus?.Id;
                            worksheet.Cells[row, col].Value = status == Enums.GetValue(DeliveryStatus.ENTREGUE) ? "OK" :
                                                              status == Enums.GetValue(DeliveryStatus.FALTOU) ? "FALTOU" :
                                                              status == Enums.GetValue(DeliveryStatus.SOLICITADO) ? "SOLICITADO" :
                                                              "A SOLICITAR";
                        }
                    }

                    Color rowColor = GetRowColor(family);
                    using (var range = worksheet.Cells[row, 1, row, 12]) // Apply color to full row
                    {
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(rowColor);
                    }

                    row++;
                }

                worksheet.Cells.AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        private DateTime GetSaturdayOfWeek(int year, int weekNumber)
        {
            DateTime firstDayOfYear = new DateTime(year, 1, 1);

            // Get the first Saturday of the year
            int daysUntilFirstSaturday = ((int)DayOfWeek.Saturday - (int)firstDayOfYear.DayOfWeek + 7) % 7;
            DateTime firstSaturday = firstDayOfYear.AddDays(daysUntilFirstSaturday);

            // Get the Saturday of the given week
            return firstSaturday.AddDays((weekNumber - 1) * 7);
        }

        private Color GetRowColor(Families family)
        {
            if (family.Familystatus?.Id == Enums.GetValue(FamilyStatus.CORTADO))
            {
                return Color.FromArgb(255, 218, 218); 
            }
            else if (family.Familystatus?.Id == Enums.GetValue(FamilyStatus.EMESPERA))
            {
                return Color.FromArgb(252, 214, 255); 
            }
            else if (string.IsNullOrEmpty(family.Name) ||
                     string.IsNullOrEmpty(family.Document) ||
                     string.IsNullOrEmpty(family.Phone))
            {
                return Color.FromArgb(249, 255, 213); 
            }

            return Color.White; // No color
        }

        public async Task<IEnumerable<MainDTO>> GetByFamilyAsync(long familyCode, string? include = null)
        {
            var list = await _mainRepository.GetByFamilyAsync(familyCode, IncludesMethods.GetIncludes(include, allowInclude));
            return list.ProjectedAsCollection<MainDTO>();
        }

        public void Dispose()
        {
            _mainRepository.Dispose();
        }
    }
}
