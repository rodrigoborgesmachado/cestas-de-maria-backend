using Newtonsoft.Json.Serialization;

namespace CestasDeMaria.Application.Helpers
{
    public class PascalCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (propertyName == null || propertyName.Length <= 1) return propertyName;
            return propertyName[0].ToString().ToUpper() + propertyName.Substring(1);
        }
    }
}
