using BGP.Utils.Repository;
using EntityFramework.Audit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BGP.Utils.Service
{
    public interface IBaseService : IDisposable
    {
        List<TDto> All<TEntity, TDto>()
            where TEntity : class;
        Task<List<TDto>> AllAsync<TEntity, TDto>()
            where TEntity : class;

        int Count<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        Task<int> CountAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        TDto Find<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        TDto FindById<TEntity, TDto>(object id)
            where TEntity : class;

        Task<TDto> FindByIdAsync<TEntity, TDto>(object id)
            where TEntity : class;

        Task<TDto> FindAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        IQueryable<TDto> Query<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        List<TDto> Filter<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        Task<List<TDto>> FilterAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        PagingResult<TDto> FilterPaged<TEntity, TDto>(PagingParams<TDto> pagingParams)
            where TEntity : class;

        Task<PagingResult<TDto>> FilterPagedAsync<TEntity, TDto>(PagingParams<TDto> pagingParams)
            where TEntity : class;

        bool Contain<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        Task<bool> ContainAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        void Create<TEntity, TDto>(TDto dto)
            where TEntity : class
            where TDto : class;

        void CreateMany<TEntity, TDto>(IEnumerable<TDto> dtos)
            where TEntity : class
            where TDto : class;

        Task CreateAsync<TEntity, TDto>(TDto dto)
            where TEntity : class
            where TDto : class;

        Task CreateManyAsync<TEntity, TDto>(IEnumerable<TDto> dtos)
            where TEntity : class
            where TDto : class;

        int DeleteById<TEntity>(object id)
            where TEntity : class;

        int DeleteMany<TEntity>(IEnumerable<object> ids)
            where TEntity : class;

        int Delete<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        Task<int> DeleteByIdAsync<TEntity>(object id)
            where TEntity : class;

        Task<int> DeleteManyAsync<TEntity>(IEnumerable<object> ids)
            where TEntity : class;

        Task<int> DeleteAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class;

        int Update<TEntity, TDto>(object id, TDto dto)
            where TEntity : class
            where TDto : class;

        Task<int> UpdateAsync<TEntity, TDto>(object id, TDto dto)
            where TEntity : class
            where TDto : class;

        void UpdateRelationship<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class;

        Task UpdateRelationshipAsync<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
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

    public interface IAdvanceBaseService : IBaseService
    {

        Task<int> UpdateSelectFieldsAsync<TEntity, TDto>(object id, TDto newDto, params Expression<Func<TEntity, Object>>[] propertiesToUpdate)
            where TEntity : class
            where TDto : class;
    }
}
