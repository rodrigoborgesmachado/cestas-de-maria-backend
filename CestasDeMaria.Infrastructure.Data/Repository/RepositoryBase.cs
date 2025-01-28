using CestasDeMaria.Infrastructure.Data.Context;
using CestasDeMaria.Infrastructure.Data.Helpers;
using CestasDeMaria.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace CestasDeMaria.Infrastructure.Data.Repository
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        protected readonly CestasDeMariaContext _currentContext;

        public RepositoryBase(CestasDeMariaContext currentContext)
        {
            _currentContext = currentContext;
        }

        public void DetachAll()
        {
            foreach (var dbEntityEntry in _currentContext.ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        public void DetachAll(string[] entities)
        {
            _currentContext.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e =>
            {
                Type entityType = e.Entity.GetType();

                if (entityType.BaseType != null && entityType.Namespace == "System.Data.Entity.DynamicProxies")
                    entityType = entityType.BaseType;

                string entityTypeName = entityType.Name;

                if (entities.Contains(entityTypeName))
                {
                    e.State = EntityState.Detached;
                }
            });
        }

        public void DetachAll(string entity)
        {
            DetachAll(new string[] { entity });
        }

        public Type FindType(string qualifiedTypeName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return _currentContext.Set<TEntity>().AsQueryable();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _currentContext.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> GetByCodeAsync(string code)
        {
            return await _currentContext.Set<TEntity>().FindAsync(code);
        }

        public TEntity GetById(string code)
        {
            return _currentContext.Set<TEntity>().Find(code);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, string[] includes = null, string orderBy = null)
        {
            var query = _currentContext.Set<TEntity>().AsQueryable();

            if (filter != null)
                query = query.Where(filter);

            return await GetAllAsync(query, includes, orderBy);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(IQueryable<TEntity> query, string[] includes = null, string orderBy = null)
        {
            includes?.ToList().ForEach(item => query = query.Include(item));

            if (!string.IsNullOrEmpty(orderBy))
            {
                var arr = orderBy.Split(':');
                query = query.OrderBy(arr[0], arr[1]);
            }

            return await query.ToListAsync();

        }

        public async Task<int> GetAllPagedTotalAsync(IQueryable<TEntity> query, string[] includes = null)
        {
            includes?.ToList().ForEach(item => query = query.Include(item));
            return await query.CountAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllPagedAsync(IQueryable<TEntity> query, int page, int quantity, string[] includes = null, string orderBy = null)
        {
            includes?.ToList().ForEach(item => query = query.Include(item));

            orderBy = orderBy ?? "Created:Asc";

            var fullName = query.GetType().GenericTypeArguments.Select(c => c.FullName).FirstOrDefault();

            string fieldOrderBy = null;

            if (!string.IsNullOrEmpty(orderBy))
            {
                var arrCheck = orderBy.Split(':');
                fieldOrderBy = arrCheck[0];
            }

            if (!string.IsNullOrEmpty(fullName))
            {
                Type objectType = FindType(fullName);

                if (objectType != null)
                {
                    var property = objectType.GetProperty(fieldOrderBy);

                    if (property == null)
                    {
                        var firstProperty = objectType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType.IsPublic).FirstOrDefault();

                        if (firstProperty != null)
                        {
                            orderBy = $"{firstProperty.Name}:Asc";
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                var arr = orderBy.Split(':');
                query = query.OrderBy(arr[0], arr[1]);
            }

            return await query.ToListPagedAsync(page, quantity);
        }

        public void Add(TEntity model)
        {
            _currentContext.CreateDefaultData(model);
            _currentContext.Set<TEntity>().Add(model);
        }

        public void AddRange(IEnumerable<TEntity> models)
        {
            foreach (var model in models)
            {
                _currentContext.CreateDefaultData(model);
                _currentContext.Set<TEntity>().Add(model);
            }
        }

        public void Update(TEntity model)
        {
            _currentContext.Entry(model).State = EntityState.Modified;
        }


        public void Merge(TEntity persisted, TEntity current)
        {
            try
            {
                _currentContext.Entry(persisted).CurrentValues.SetValues(current);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Merge<T>(T persisted, T current) where T : class
        {
            _currentContext.Entry(persisted).CurrentValues.SetValues(current);
        }

        public void Remove(TEntity model)
        {
            _currentContext.Entry(model).State = EntityState.Deleted;
            _currentContext.Set<TEntity>().Remove(model);
        }

        public void RemoveRange(List<TEntity> models)
        {
            foreach (var item in models)
            {
                _currentContext.Entry(item).State = EntityState.Deleted;
            }
            _currentContext.Set<TEntity>().RemoveRange(models);
        }

        public void Commit()
        {
            _currentContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _currentContext.SaveChangesAsync();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _currentContext.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public async Task CallProcedure(string proc, Dictionary<string, object> parameters)
        {
            if (parameters != null)
            {
                var parameterString = string.Join(", ", parameters?.Keys);
                var valueString = string.Join(", ", parameters?.Values?.Select(v => v.ToString()));
                var sqlCommand = $"EXEC dbo.{proc} {parameterString} {valueString}";

                await _currentContext.Database.ExecuteSqlRawAsync(sqlCommand);
            }
            else
            {
                await _currentContext.Database.ExecuteSqlRawAsync($"EXEC dbo.{proc}");
            }
        }
    }
}
