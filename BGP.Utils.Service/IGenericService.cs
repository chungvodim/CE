using BGP.Utils.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BGP.Utils.Service
{
    public interface IGenericService<TEntity> : IBaseService
        where TEntity : class
    {
        List<TDto> All<TDto>();

        Task<List<TDto>> AllAsync<TDto>();

        int Count<TDto>(Expression<Func<TDto, bool>> predicate);

        Task<int> CountAsync<TDto>(Expression<Func<TDto, bool>> predicate);

        TDto Find<TDto>(Expression<Func<TDto, bool>> predicate);

        TDto Find<TDto>(object id);

        Task<TDto> FindByIdAsync<TDto>(object id);

        Task<TDto> FindAsync<TDto>(Expression<Func<TDto, bool>> predicate);

        List<TDto> Filter<TDto>(Expression<Func<TDto, bool>> predicate);

        Task<List<TDto>> FilterAsync<TDto>(Expression<Func<TDto, bool>> predicate);

        PagingResult<TDto> FilterPaged<TDto>(PagingParams<TDto> pagingParams);

        Task<PagingResult<TDto>> FilterPagedAsync<TDto>(PagingParams<TDto> pagingParams);

        bool Contain<TDto>(Expression<Func<TDto, bool>> predicate);

        Task<bool> ContainAsync<TDto>(Expression<Func<TDto, bool>> predicate);

        void Create<TDto>(TDto dto) where TDto : class;

        void CreateMany<TDto>(IEnumerable<TDto> dtos) where TDto : class;

        Task CreateAsync<TDto>(TDto dto) where TDto : class;

        Task CreateManyAsync<TDto>(IEnumerable<TDto> dtos) where TDto : class;

        int DeleteById(object id);

        int DeleteMany(IEnumerable<object> ids);

        int Delete<TDto>(Expression<Func<TDto, bool>> predicate);

        Task<int> DeleteByIdAsync(object id);

        Task<int> DeleteManyAsync(IEnumerable<object> ids);

        Task<int> DeleteAsync<TDto>(Expression<Func<TDto, bool>> predicate);

        int Update<TDto>(object id, TDto dto) where TDto : class;

        Task<int> UpdateAsync<TDto>(object id, TDto dto) where TDto : class;

        void UpdateRelationship<TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TRelationship : class;

        Task UpdateRelationshipAsync<TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TRelationship : class;
    }
}
