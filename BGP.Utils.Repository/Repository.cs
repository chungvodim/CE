using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Data;
using EntityFramework.Audit;
using EntityFramework.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BGP.Utils.Repository.EntityFramework
{
    public class Repository : IRepository
    {
        protected DbContext _dbContext;
        private AuditLogger _auditLog;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual IQueryable<TEntity> All<TEntity>()
            where TEntity : class
        {
            return _dbContext.Set<TEntity>();
        }

        public virtual int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return _dbContext.Set<TEntity>().Count(predicate);
        }

        public async virtual Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().CountAsync(predicate);
        }

        public virtual TEntity Find<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return _dbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        public virtual TEntity FindById<TEntity>(object id)
            where TEntity : class
        {
            return _dbContext.Set<TEntity>().Find(id);
        }

        public async virtual Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(predicate);
        }

        public async virtual Task<TEntity> FindByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().FindAsync(id);
        }

        public virtual IQueryable<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return _dbContext.Set<TEntity>().Where(predicate);
        }

        public virtual PagingResult<TEntity> FilterPaged<TEntity>(PagingParams<TEntity> pagingParams)
            where TEntity : class
        {
            if (pagingParams == null)
            {
                throw new ArgumentNullException("pagingParams");
            }

            var result = new PagingResult<TEntity>()
            {
                CurrentPage = pagingParams.Page,
                PageSize = pagingParams.PageSize
            };

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (pagingParams.Predicate != null)
            {
                query = query.Where(pagingParams.Predicate);
            }

            result.ItemCount = query.Count();

            // Ordering
            if (pagingParams.SortBy != null)
            {
                query = query.OrderBy(pagingParams.SortBy);
            }
            else
            {
                query = query.OrderBy(pagingParams.SortField + (pagingParams.IsAscending ? " asc" : " desc"));

                // Skipping only work after ordering
                if (pagingParams.StartingIndex > 0)
                {
                    query = query.Skip(pagingParams.StartingIndex);
                }
            }

            if (pagingParams.PageSize > 0)
            {
                query = query.Take(pagingParams.PageSize);
            }

            result.Items = query.ToList();

            return result;
        }

        public async virtual Task<PagingResult<TEntity>> FilterPagedAsync<TEntity>(PagingParams<TEntity> pagingParams)
            where TEntity : class
        {
            if (pagingParams == null)
            {
                throw new ArgumentNullException("pagingParams");
            }

            var result = new PagingResult<TEntity>()
            {
                CurrentPage = pagingParams.Page,
                PageSize = pagingParams.PageSize
            };

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (pagingParams.Predicate != null)
            {
                query = query.Where(pagingParams.Predicate);
            }

            result.ItemCount = await query.CountAsync();

            // Ordering
            if (pagingParams.SortBy != null)
            {
                query = query.OrderBy(pagingParams.SortBy);
            }
            else
            {
                query = query.OrderBy(pagingParams.SortField + (pagingParams.IsAscending ? " asc" : " desc"));

                // Skipping only work after ordering
                if (pagingParams.StartingIndex > 0)
                {
                    query = query.Skip(pagingParams.StartingIndex);
                }
            }

            if (pagingParams.PageSize > 0)
            {
                query = query.Take(pagingParams.PageSize);
            }

            result.Items = await query.ToListAsync();

            return result;
        }

        public bool Contain<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return _dbContext.Set<TEntity>().Any(predicate);
        }

        public async Task<bool> ContainAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return await _dbContext.Set<TEntity>().AnyAsync(predicate);
        }

        public virtual void Create<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);

            _dbContext.SaveChanges();
        }

        public virtual void CreateMany<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().AddRange(entities);

            _dbContext.SaveChanges();
        }

        public async virtual Task CreateAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);

            await _dbContext.SaveChangesAsync();
        }

        public async virtual Task CreateManyAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public virtual int DeleteById<TEntity>(object id)
            where TEntity : class
        {
            var entity = FindById<TEntity>(id);

            _dbContext.Set<TEntity>().Remove(entity);

            return _dbContext.SaveChanges();
        }

        public virtual int DeleteMany<TEntity>(IEnumerable<object> ids)
            where TEntity : class
        {
            foreach (var id in ids)
            {
                var entity = FindById<TEntity>(id);

                if (entity is ICascadeDelete)
                {
                    (entity as ICascadeDelete).OnDelete();
                }

                _dbContext.Set<TEntity>().Remove(entity);
            }
            return _dbContext.SaveChanges();
        }

        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            var entities = Filter<TEntity>(predicate).ToArray();

            foreach (var entity in entities)
            {
                if (entity is ICascadeDelete)
                {
                    (entity as ICascadeDelete).OnDelete();
                }
                _dbContext.Set<TEntity>().Remove(entity);
            }
            return _dbContext.SaveChanges();
        }

        public async virtual Task<int> DeleteByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            var entity = await FindByIdAsync<TEntity>(id);

            _dbContext.Set<TEntity>().Remove(entity);

            return await _dbContext.SaveChangesAsync();
        }

        public async virtual Task<int> DeleteManyAsync<TEntity>(IEnumerable<object> ids)
            where TEntity : class
        {
            foreach (var id in ids)
            {
                var entity = await FindByIdAsync<TEntity>(id);

                if (entity is ICascadeDelete)
                {
                    (entity as ICascadeDelete).OnDelete();
                }

                _dbContext.Set<TEntity>().Remove(entity);
            }
            return await _dbContext.SaveChangesAsync();
        }

        public async virtual Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            var entities = await Filter<TEntity>(predicate).ToArrayAsync();

            foreach (var entity in entities)
            {
                if (entity is ICascadeDelete)
                {
                    (entity as ICascadeDelete).OnDelete();
                }
                _dbContext.Set<TEntity>().Remove(entity);
            }
            return await _dbContext.SaveChangesAsync();
        }

        public virtual int Update<TEntity>(object id, TEntity entity)
            where TEntity : class
        {
            var entry = _dbContext.Entry(entity);

            return _dbContext.SaveChanges();
        }

        public void UpdatRelationship<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class
        {
            var entity = FindById<TEntity>(entityId);

            var filteredRelationship = Filter(relationShipFilter).ToList();

            var existingRelationship = relationshipSelector.Invoke(entity);

            var removingRelationship = new List<TRelationship>();
            var newRelationship = new List<TRelationship>();

            foreach (var relationship in existingRelationship)
            {
                if (!filteredRelationship.Contains(relationship))
                {
                    removingRelationship.Add(relationship);
                }
            }
            foreach (var relationship in filteredRelationship)
            {
                if (!existingRelationship.Contains(relationship))
                {
                    newRelationship.Add(relationship);
                }
            }

            removingRelationship.ForEach(x => existingRelationship.Remove(x));
            newRelationship.ForEach(x => existingRelationship.Add(x));

            _dbContext.SaveChanges();
        }

        public async Task UpdatRelationshipAsync<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class
        {
            var entity = await FindByIdAsync<TEntity>(entityId);

            var filteredRelationship = await Filter(relationShipFilter).ToListAsync();

            var existingRelationship = relationshipSelector.Invoke(entity);

            var removingRelationship = new List<TRelationship>();
            var newRelationship = new List<TRelationship>();

            foreach (var relationship in existingRelationship)
            {
                if (!filteredRelationship.Contains(relationship))
                {
                    removingRelationship.Add(relationship);
                }
            }
            foreach (var relationship in filteredRelationship)
            {
                if (!existingRelationship.Contains(relationship))
                {
                    newRelationship.Add(relationship);
                }
            }

            removingRelationship.ForEach(x => existingRelationship.Remove(x));
            newRelationship.ForEach(x => existingRelationship.Add(x));

            await _dbContext.SaveChangesAsync();
        }

        public async virtual Task<int> UpdateAsync<TEntity>(object id, TEntity entity)
            where TEntity : class
        {
            var entry = _dbContext.Entry(entity);

            return await _dbContext.SaveChangesAsync();
        }

        public virtual int ExecuteNonQuery(string query, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand(query, parameters);
        }

        public virtual TResult ExecuteReader<TResult>(string query, params object[] parameters)
        {
            return _dbContext.Database.SqlQuery<TResult>(query, parameters).FirstOrDefault();
        }

        public TDbContext GetDbContext<TDbContext>()
            where TDbContext : class
        {
            return this._dbContext as TDbContext;
        }

        public virtual ITransaction BeginTransaction()
        {
            return new Transaction(_dbContext.Database.BeginTransaction());
        }

        public virtual ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new Transaction(_dbContext.Database.BeginTransaction(isolationLevel));
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }

        public void BeginAuditLog()
        {
            _auditLog = _dbContext.BeginAudit();
        }

        public string GetLastLog()
        {
            if (_auditLog == null)
                throw new InvalidOperationException("Run BeginAuditLog first");

            var lastLog = _auditLog.LastLog;
            if (lastLog != null && lastLog.Entities.Count != 0)
            {
                var list = new List<ChangeLog>();
                foreach (var entity in lastLog.Entities)
                {
                    var properties = entity.Current.GetType().GetProperties();
                    list.Add(new ChangeLog
                    {
                        AuditAction = entity.Action,
                        Keys = entity.Keys.Select(m => new ChangeLogKey
                        {
                            Name = m.Name,
                            Value = properties.First(p => p.Name == m.Name).GetValue(entity.Current) != null ? properties.First(p => p.Name == m.Name).GetValue(entity.Current).ToString() : ""
                        }),
                        Properties = entity.Properties.Select(m => new ChangeLogProperty
                        {
                            Name = m.Name,
                            Old = m.Original,
                            New = properties.First(p => p.Name == m.Name).GetValue(entity.Current) != null ? properties.First(p => p.Name == m.Name).GetValue(entity.Current).ToString() : ""
                        })
                    });
                }

                var json = JsonConvert.SerializeObject(list);
                return json;
            }

            return null;
        }
    }
}
