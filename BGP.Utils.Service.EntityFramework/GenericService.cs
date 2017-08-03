using BGP.Utils.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BGP.Utils.Service.EntityFramework
{
    public class GenericService<TEntity> : BaseService, IGenericService<TEntity>
        where TEntity : class
    {
        public GenericService(BGP.Utils.Repository.EntityFramework.Repository repository)
            : base(repository) { }

        #region IGenericService<TEntity,TDto,TKey> Members

        public virtual List<TDto> All<TDto>()
        {
            return base.All<TEntity, TDto>();
        }

        public async virtual Task<List<TDto>> AllAsync<TDto>()
        {
            return await base.AllAsync<TEntity, TDto>();
        }

        public virtual int Count<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return base.Count<TEntity, TDto>(predicate);
        }

        public virtual async Task<int> CountAsync<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return await base.CountAsync<TEntity, TDto>(predicate);
        }

        public virtual TDto Find<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return base.Find<TEntity, TDto>(predicate);
        }

        public virtual TDto Find<TDto>(object id)
        {
            return base.FindById<TEntity, TDto>(id);
        }

        public virtual async Task<TDto> FindByIdAsync<TDto>(object id)
        {
            return await base.FindByIdAsync<TEntity, TDto>(id);
        }

        public virtual async Task<TDto> FindAsync<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return await base.FindAsync<TEntity, TDto>(predicate);
        }

        public virtual List<TDto> Filter<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return base.Filter<TEntity, TDto>(predicate);
        }

        public async virtual Task<List<TDto>> FilterAsync<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return await base.FilterAsync<TEntity, TDto>(predicate);
        }

        public virtual PagingResult<TDto> FilterPaged<TDto>(PagingParams<TDto> pagingParams)
        {
            return base.FilterPaged<TEntity, TDto>(pagingParams);
        }

        public async virtual Task<PagingResult<TDto>> FilterPagedAsync<TDto>(PagingParams<TDto> pagingParams)
        {
            return await base.FilterPagedAsync<TEntity, TDto>(pagingParams);
        }

        public virtual bool Contain<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return base.Contain<TEntity, TDto>(predicate);
        }

        public virtual async Task<bool> ContainAsync<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return await base.ContainAsync<TEntity, TDto>(predicate);
        }

        public virtual void Create<TDto>(TDto dto) where TDto : class
        {
            base.Create<TEntity, TDto>(dto);
        }

        public virtual void CreateMany<TDto>(IEnumerable<TDto> dtos) where TDto : class
        {
            base.CreateMany<TEntity, TDto>(dtos);
        }

        public virtual async Task CreateAsync<TDto>(TDto dto) where TDto : class
        {
            await base.CreateAsync<TEntity, TDto>(dto);
        }

        public virtual async Task CreateManyAsync<TDto>(IEnumerable<TDto> dtos) where TDto : class
        {
            await base.CreateManyAsync<TEntity, TDto>(dtos);
        }

        public virtual int DeleteById(object id)
        {
            return base.DeleteById<TEntity>(id);
        }

        public virtual int DeleteMany(IEnumerable<object> ids)
        {
            return base.DeleteMany<TEntity>(ids);
        }

        public virtual int Delete<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return base.Delete<TEntity, TDto>(predicate);
        }

        public virtual async Task<int> DeleteByIdAsync(object id)
        {
            return await base.DeleteByIdAsync<TEntity>(id);
        }

        public virtual async Task<int> DeleteManyAsync(IEnumerable<object> ids)
        {
            return await base.DeleteByIdAsync<TEntity>(ids);
        }

        public virtual async Task<int> DeleteAsync<TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return await base.DeleteAsync<TEntity, TDto>(predicate);
        }

        public virtual int Update<TDto>(object id, TDto dto) where TDto : class
        {
            return base.Update<TEntity, TDto>(id, dto);
        }

        public virtual async Task<int> UpdateAsync<TDto>(object id, TDto dto) where TDto : class
        {
            return await base.UpdateAsync<TEntity, TDto>(id, dto);
        }

        public void UpdateRelationship<TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector) where TRelationship : class
        {
            base.UpdateRelationship(entityId, relationShipFilter, relationshipSelector);
        }

        public async Task UpdateRelationshipAsync<TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector) where TRelationship : class
        {
            await base.UpdateRelationshipAsync(entityId, relationShipFilter, relationshipSelector);
        }

        #endregion
    }

}
