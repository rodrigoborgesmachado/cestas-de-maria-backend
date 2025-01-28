namespace CestasDeMaria.Infrastructure.CrossCutting.Adapter
{
    public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
    {
        public ITypeAdapter Create()
        {
            return new AutomapperTypeAdapter();
        }
    }
}
