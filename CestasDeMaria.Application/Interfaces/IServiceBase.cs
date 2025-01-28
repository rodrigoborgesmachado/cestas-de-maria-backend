namespace CestasDeMaria.Application.Interfaces
{
    public interface IServiceBase<T> where T : class
    {
        Task<string> UploadReport(List<T> list);
    }
}
