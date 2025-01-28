using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CestasDeMaria.Presentation.Api.App_Start
{
    public class SwaggerControllerDebug : IDocumentFilter
    {
        void IDocumentFilter.Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths
                .OrderBy(pair => !pair.Key.Contains("Token"))
                .ThenBy(pair => pair.Key)
                .ToList();

            var list = new OpenApiPaths();
            foreach (var path in paths)
            {
                list.Add(path.Key, path.Value);
            }

            swaggerDoc.Paths = list;
        }
    }
}
