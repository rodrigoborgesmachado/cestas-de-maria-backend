using System.Linq.Expressions;

namespace CestasDeMaria.Domain.Interfaces.Repository
{
    public interface IRepositoryBase<TEntity> : IDisposable where TEntity : class
    {
        void Add(TEntity model);
        void AddRange(IEnumerable<TEntity> models);

        IQueryable<TEntity> GetQueryable();

        Task<TEntity> GetByIdAsync(int id);

        Task<TEntity> GetByCodeAsync(string code);
        TEntity GetById(string code);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, string[] includes = null, string orderBy = null);

        Task<IEnumerable<TEntity>> GetAllAsync(IQueryable<TEntity> query, string[] includes = null, string orderBy = null);

        Task<int> GetAllPagedTotalAsync(IQueryable<TEntity> query, string[] includes = null);

        Task<IEnumerable<TEntity>> GetAllPagedAsync(IQueryable<TEntity> query, int page, int quantity, string[] includes = null, string orderBy = null);

        void Update(TEntity model);

        void Remove(TEntity model);

        void RemoveRange(List<TEntity> models);

        void Merge(TEntity persisted, TEntity current);

        void Merge<T>(T persisted, T current) where T : class;

        void Commit();

        Task CommitAsync();

        void DetachAll();

        Task CallProcedure(string proc, Dictionary<string, object> parameters);
    }
}
