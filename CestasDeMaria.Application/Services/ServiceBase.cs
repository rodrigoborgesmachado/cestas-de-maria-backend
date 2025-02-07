using CestasDeMaria.Application.Interfaces;
using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace CestasDeMaria.Application.Services
{
    public class ServiceBase<T> : IServiceBase<T> where T : class
    {
        public readonly IBlobStorageService _iBlobStorageService;
        public readonly Settings _settings;

        public ServiceBase(IBlobStorageService iBlobStorageService, IOptions<Settings> options)
        {
            _iBlobStorageService = iBlobStorageService;
            _settings = options.Value;
        }

        public async Task<string> UploadReport(List<T> objects)
        {
            if (objects == null || objects.Count == 0)
                return string.Empty;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var list = ConvertToDynamic(objects);

            var properties = ((object)list[0]).GetType().GetProperties();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(((object)list[0]).GetType().Name);
                int row = 1;
                for(int i = 0; i < properties.Count(); i++) 
                {
                    worksheet.Cells[1, i+row].Value = properties[i].Name;
                }
                
                row++;
                for(int i = 0;i < list.Count(); i++)
                {
                    var values = properties.Select(p => p.GetValue(list[i], null)?.ToString() ?? string.Empty).ToArray();
                    for(int j = 0; j < values.Count(); j++)
                    {
                        if (properties[j].PropertyType.FullName.Contains("System.Byte"))
                        {
                            worksheet.Cells[i + row, j + 1].Value = values[j].Equals("1") ? "True" : "False";
                        }
                        else
                        {
                            worksheet.Cells[i+row, j+1].Value = values[j];
                        }
                    }
                }
                
                worksheet.Cells.AutoFitColumns();
                string fileName = GenerateReportName(((object)list[0]).GetType().Name);
                return await _iBlobStorageService.UploadFileAsync(package.GetAsByteArray(), fileName);
            }
        }

        private List<dynamic> ConvertToDynamic<T>(List<T> list)
        {
            return list.Select(item => (dynamic)item).ToList();
        }

        private string GenerateReportName(string type)
        {
            int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string fileName = $"{type}${unixTimestamp}.xlsx";

            return fileName;
        }
    }
}
