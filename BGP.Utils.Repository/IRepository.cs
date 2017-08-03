using EntityFramework.Audit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BGP.Utils.Repository
{
    public interface IRepository : IDisposable
    {
        IQueryable<TEntity> All<TEntity>()
            where TEntity : class;

        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        TEntity Find<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        TEntity FindById<TEntity>(object id)
            where TEntity : class;

        Task<TEntity> FindByIdAsync<TEntity>(object id)
            where TEntity : class;

        Task<TEntity> FindAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        IQueryable<TEntity> Filter<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        PagingResult<TEntity> FilterPaged<TEntity>(PagingParams<TEntity> pagingParams)
            where TEntity : class;

        Task<PagingResult<TEntity>> FilterPagedAsync<TEntity>(PagingParams<TEntity> pagingParams)
            where TEntity : class;

        bool Contain<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        Task<bool> ContainAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        void Create<TEntity>(TEntity entity)
            where TEntity : class;

        void CreateMany<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;

        Task CreateAsync<TEntity>(TEntity entity)
            where TEntity : class;

        Task CreateManyAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class;

        int DeleteById<TEntity>(object id)
            where TEntity : class;

        int DeleteMany<TEntity>(IEnumerable<object> ids)
            where TEntity : class;

        int Delete<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        Task<int> DeleteByIdAsync<TEntity>(object id)
            where TEntity : class;

        Task<int> DeleteManyAsync<TEntity>(IEnumerable<object> ids)
            where TEntity : class;

        Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        int Update<TEntity>(object id, TEntity entity)
            where TEntity : class;

        Task<int> UpdateAsync<TEntity>(object id, TEntity entity)
            where TEntity : class;

        void UpdatRelationship<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class;

        Task UpdatRelationshipAsync<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class;

        int ExecuteNonQuery(string query, params object[] parameters);

        TResult ExecuteReader<TResult>(string query, params object[] parameters);

        TContext GetDbContext<TContext>() where TContext : class;

        ITransaction BeginTransaction();

        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        void BeginAuditLog();
        string GetLastLog();
    }
}
