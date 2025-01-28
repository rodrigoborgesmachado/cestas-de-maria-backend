namespace CestasDeMaria.Infrastructure.CrossCutting.Adapter
{
    public interface ITypeAdapterFactory
    {
        /// <summary>
        ///   Create a type adater
        /// </summary>
        /// <returns>The created ITypeAdapter</returns>
        ITypeAdapter Create();
    }
}
