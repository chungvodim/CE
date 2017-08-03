using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using System.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using EntityFramework.Audit;

namespace BGP.Utils.Repository.Mongo
{
    public class Repository : IRepository
    {
        protected IMongoDatabase _dbContext;

        public Repository(IMongoDatabase dbContext)
        {
            _dbContext = dbContext;
        }

        #region MongoSpecific

        /// <summary>
        /// mongo collection
        /// </summary>
        public IMongoCollection<TEntity> Collection<TEntity>() where TEntity : class
        {
            return _dbContext.GetCollection<TEntity>(CollectionNameAttribute.GetCollectionName<TEntity>());
        }

        /// <summary>
        /// filter for collection
        /// </summary>
        public FilterDefinitionBuilder<TEntity> Filter<TEntity>() where TEntity : class
        {
            return Builders<TEntity>.Filter;
        }

        /// <summary>
        /// projector for collection
        /// </summary>
        public ProjectionDefinitionBuilder<TEntity> Project<TEntity>() where TEntity : class
        {
            return Builders<TEntity>.Projection;
        }

        /// <summary>
        /// updater for collection
        /// </summary>
        public UpdateDefinitionBuilder<TEntity> Updater<TEntity>() where TEntity : class
        {
            return Builders<TEntity>.Update;
        }

        /// <summary>
        /// sorter for collection
        /// </summary>
        public SortDefinitionBuilder<TEntity> Sorter<TEntity>() where TEntity : class
        {
            return Builders<TEntity>.Sort;
        }

        private FilterDefinition<TEntity> FilterById<TEntity>(object id) where TEntity : class
        {
            return Filter<TEntity>().Eq("_id", ObjectId.Parse(id.ToString()));
        }

        private FilterDefinition<TEntity> FilterByIds<TEntity>(IEnumerable<object> ids) where TEntity : class
        {
            return Filter<TEntity>().AnyIn("_id", ids.Select(x => ObjectId.Parse(x.ToString())));
        }
        
        #endregion MongoSpecific

        public virtual IQueryable<TEntity> All<TEntity>()
            where TEntity : class
        {
            return Collection<TEntity>().AsQueryable();
        }

        public virtual int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return (int)Collection<TEntity>().Count(predicate);
        }

        public async virtual Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return (int)await Collection<TEntity>().CountAsync(predicate);
        }

        public virtual TEntity Find<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return Collection<TEntity>().Find(predicate).FirstOrDefault();
        }

        public virtual TEntity FindById<TEntity>(object id)
            where TEntity : class
        {
            return Collection<TEntity>().Find(FilterById<TEntity>(id)).FirstOrDefault();
        }

        public async virtual Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return await Collection<TEntity>().Find(predicate).FirstOrDefaultAsync();
        }

        public async virtual Task<TEntity> FindByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            return await Collection<TEntity>().Find<TEntity>(FilterById<TEntity>(id)).FirstOrDefaultAsync();
        }

        public virtual IQueryable<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return Collection<TEntity>().AsQueryable().Where(predicate);
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

            var query = Collection<TEntity>()
                .Find(pagingParams.Predicate);

            result.ItemCount = (int)query.Count();

            if (pagingParams.SortBy != null)
            {
                query = pagingParams.IsAscending ? query.SortBy(pagingParams.SortBy) : query.SortByDescending(pagingParams.SortBy);
            }
            else
            {
                query = query.Sort(pagingParams.IsAscending ? Sorter<TEntity>().Ascending(pagingParams.SortField) : Sorter<TEntity>().Descending(pagingParams.SortField));
            }

            if (pagingParams.StartingIndex > 0)
            {
                query = query.Skip(pagingParams.StartingIndex);
            }

            if (pagingParams.PageSize > 0)
            {
                query = query.Limit(pagingParams.PageSize);
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

            var query = Collection<TEntity>()
                .Find(pagingParams.Predicate);

            result.ItemCount = (int)await query.CountAsync();

            if (pagingParams.SortBy != null)
            {
                query = pagingParams.IsAscending ? query.SortBy(pagingParams.SortBy) : query.SortByDescending(pagingParams.SortBy);
            }
            else
            {
                query = query.Sort(pagingParams.IsAscending ? Sorter<TEntity>().Ascending(pagingParams.SortField) : Sorter<TEntity>().Descending(pagingParams.SortField));
            }

            if (pagingParams.StartingIndex > 0)
            {
                query = query.Skip(pagingParams.StartingIndex);
            }

            if (pagingParams.PageSize > 0)
            {
                query = query.Limit(pagingParams.PageSize);
            }

            result.Items = await query.ToListAsync();

            return result;
        }

        public bool Contain<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return Collection<TEntity>().Find(predicate).Any();
        }

        public async Task<bool> ContainAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return await Collection<TEntity>().Find(predicate).AnyAsync();
        }

        public virtual void Create<TEntity>(TEntity entity)
            where TEntity : class
        {
            Collection<TEntity>().InsertOne(entity);
        }

        public virtual void CreateMany<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            Collection<TEntity>().InsertMany(entities);
        }

        public async virtual Task CreateAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            await Collection<TEntity>().InsertOneAsync(entity);
        }

        public async virtual Task CreateManyAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            await Collection<TEntity>().InsertManyAsync(entities);
        }

        public virtual int DeleteById<TEntity>(object id)
            where TEntity : class
        {
            return (int)Collection<TEntity>().DeleteOne(FilterById<TEntity>(id)).DeletedCount;
        }

        public virtual int DeleteMany<TEntity>(IEnumerable<object> ids)
            where TEntity : class
        {
            return (int)Collection<TEntity>().DeleteMany(FilterByIds<TEntity>(ids)).DeletedCount;
        }

        public virtual int Delete<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return (int)Collection<TEntity>().DeleteMany(predicate).DeletedCount;
        }

        public async virtual Task<int> DeleteByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            return (int)(await Collection<TEntity>().DeleteOneAsync(FilterById<TEntity>(id))).DeletedCount;
        }

        public async virtual Task<int> DeleteManyAsync<TEntity>(IEnumerable<object> ids)
            where TEntity : class
        {
            return (int)(await Collection<TEntity>().DeleteManyAsync(FilterByIds<TEntity>(ids))).DeletedCount;
        }

        public async virtual Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            return (int)(await Collection<TEntity>().DeleteManyAsync(predicate)).DeletedCount;
        }

        public virtual int Update<TEntity>(object id, TEntity entity)
            where TEntity : class
        {
            return (int)Collection<TEntity>().ReplaceOne(FilterById<TEntity>(id), entity).ModifiedCount;
        }

        public async virtual Task<int> UpdateAsync<TEntity>(object id, TEntity entity)
            where TEntity : class
        {
            return (int)(await Collection<TEntity>().ReplaceOneAsync(FilterById<TEntity>(id), entity)).ModifiedCount;
        }

        public void UpdatRelationship<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class
        {
            throw new NotImplementedException();
        }

        public Task UpdatRelationshipAsync<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class
        {
            throw new NotImplementedException();
        }

        public virtual int ExecuteNonQuery(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual TResult ExecuteReader<TResult>(string query, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public TDbContext GetDbContext<TDbContext>()
            where TDbContext : class
        {
            return this._dbContext as TDbContext;
        }

        public virtual ITransaction BeginTransaction()
        {
            return new MongoTransaction();
        }

        public virtual ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new MongoTransaction();
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext = null;
            }
        }

        public void BeginAuditLog()
        {
            throw new NotImplementedException();
        }

        public string GetLastLog()
        {
            throw new NotImplementedException();
        }
    }
}
