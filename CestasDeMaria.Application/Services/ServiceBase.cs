using CestasDeMaria.Application.Interfaces;
using CestasDeMaria.Domain.Interfaces.Services;
using CestasDeMaria.Domain.ModelClasses;
using Microsoft.Extensions.Options;
using System.Text;

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

            var list = ConvertToDynamic(objects);

            StringBuilder builder = new StringBuilder();

            // Get the properties of the first object in the list (assuming all objects have the same structure)
            var properties = ((object)list[0]).GetType().GetProperties();

            // Add the header line (property names)
            builder.AppendLine(string.Join(";", properties.Select(p => p.Name)));

            // Add the data lines
            foreach (var item in list)
            {
                var values = properties.Select(p => p.GetValue(item, null)?.ToString() ?? string.Empty);
                builder.AppendLine(string.Join(";", values));
            }

            // Upload logic or return the CSV content
            string csvContent = builder.ToString();

            string fileName = GenerateReportName(((object)list[0]).GetType().Name);

            return await _iBlobStorageService.UploadFileAsync(fileName, csvContent);
        }

        private List<dynamic> ConvertToDynamic<T>(List<T> list)
        {
            return list.Select(item => (dynamic)item).ToList();
        }

        private string GenerateReportName(string type)
        {
            int unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string fileName = $"{type}${unixTimestamp}.csv";

            return fileName;
        }
    }
}
