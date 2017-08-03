using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BGP.Utils.Repository;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Data;
using System.Data.Entity;
using BGP.Utils.Repository.EntityFramework;
using EntityFramework.Audit;
using System.Reflection;

namespace BGP.Utils.Service.EntityFramework
{
    public class BaseService : IAdvanceBaseService
    {
        protected BGP.Utils.Repository.EntityFramework.Repository _repository;

        public BaseService(BGP.Utils.Repository.EntityFramework.Repository repository)
        {
            _repository = repository;
        }

        #region IBaseService Members

        public virtual List<TDto> All<TEntity, TDto>()
            where TEntity : class
        {
            return _repository.All<TEntity>().ProjectTo<TDto>().ToList();
        }

        public async virtual Task<List<TDto>> AllAsync<TEntity, TDto>()
            where TEntity : class
        {
            return await _repository.All<TEntity>().ProjectTo<TDto>().ToListAsync();
        }

        public virtual int Count<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Count(Mapper.Map<Expression<Func<TEntity, bool>>>(predicate));
        }

        public virtual async Task<int> CountAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.CountAsync(Mapper.Map<Expression<Func<TEntity, bool>>>(predicate));
        }

        public virtual TDto Find<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.All<TEntity>().ProjectTo<TDto>().FirstOrDefault(predicate);
        }

        public virtual TDto FindById<TEntity, TDto>(object id)
            where TEntity : class
        {
            var entity = _repository.FindById<TEntity>(id);
            var dto = Mapper.Map<TEntity, TDto>(entity);
            return dto;
        }

        public virtual async Task<TDto> FindByIdAsync<TEntity, TDto>(object id)
            where TEntity : class
        {
            return Mapper.Map<TEntity, TDto>(await _repository.FindByIdAsync<TEntity>(id));
        }

        public virtual async Task<TDto> FindAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.All<TEntity>().ProjectTo<TDto>().FirstOrDefaultAsync(predicate);
        }

        public virtual IQueryable<TDto> Query<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.All<TEntity>().ProjectTo<TDto>().Where(predicate);
        }

        public virtual List<TDto> Filter<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.All<TEntity>().ProjectTo<TDto>().Where(predicate).ToList();
        }

        public async virtual Task<List<TDto>> FilterAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.All<TEntity>().ProjectTo<TDto>().Where(predicate).ToListAsync();
        }

        public virtual PagingResult<TDto> FilterPaged<TEntity, TDto>(PagingParams<TDto> pagingParams)
            where TEntity : class
        {
            if (pagingParams == null)
            {
                throw new ArgumentNullException("pagingParams");
            }

            var result = new PagingResult<TDto>()
            {
                CurrentPage = pagingParams.Page,
                PageSize = pagingParams.PageSize
            };

            IQueryable<TDto> query = _repository.All<TEntity>().ProjectTo<TDto>();

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

        public async virtual Task<PagingResult<TDto>> FilterPagedAsync<TEntity, TDto>(PagingParams<TDto> pagingParams)
            where TEntity : class
        {
            if (pagingParams == null)
            {
                throw new ArgumentNullException("pagingParams");
            }

            var result = new PagingResult<TDto>()
            {
                CurrentPage = pagingParams.Page,
                PageSize = pagingParams.PageSize
            };

            IQueryable<TDto> query = _repository.All<TEntity>().ProjectTo<TDto>();

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

        public virtual bool Contain<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Contain(Mapper.Map<Expression<Func<TEntity, bool>>>(predicate));
        }

        public virtual async Task<bool> ContainAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.ContainAsync(Mapper.Map<Expression<Func<TEntity, bool>>>(predicate));
        }

        public virtual void Create<TEntity, TDto>(TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = Mapper.Map<TDto, TEntity>(dto);

            _repository.Create<TEntity>(entity);

            MapKeyBack(entity, dto);
        }

        public virtual void CreateMany<TEntity, TDto>(IEnumerable<TDto> dtos)
            where TEntity : class
            where TDto : class
        {
            var entities = Mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(dtos);

            _repository.CreateMany<TEntity>(entities);

            var listEntities = entities.ToList();
            var listDtos = dtos.ToList();
            for (var i = 0; i < listEntities.Count; i++)
            {
                MapKeyBack(listEntities[i], listDtos[i]);
            }
        }

        public async virtual Task CreateAsync<TEntity, TDto>(TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = Mapper.Map<TDto, TEntity>(dto);

            await _repository.CreateAsync<TEntity>(entity);

            MapKeyBack(entity, dto);
        }

        public async virtual Task CreateManyAsync<TEntity, TDto>(IEnumerable<TDto> dtos)
            where TEntity : class
            where TDto : class
        {
            var entities = Mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(dtos);

            await _repository.CreateManyAsync<TEntity>(entities);

            var listEntities = entities.ToList();
            var listDtos = dtos.ToList();
            for (var i = 0; i < listEntities.Count; i++)
            {
                MapKeyBack(listEntities[i], listDtos[i]);
            }
        }

        public virtual int DeleteById<TEntity>(object id)
            where TEntity : class
        {
            return _repository.DeleteById<TEntity>(id);
        }

        public virtual int DeleteMany<TEntity>(IEnumerable<object> ids)
            where TEntity : class
        {
            return _repository.DeleteById<TEntity>(ids);
        }

        public virtual int Delete<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Delete<TEntity>(Mapper.Map<Expression<Func<TEntity, bool>>>(predicate));
        }

        public virtual async Task<int> DeleteByIdAsync<TEntity>(object id)
            where TEntity : class
        {
            return await _repository.DeleteByIdAsync<TEntity>(id);
        }

        public virtual async Task<int> DeleteManyAsync<TEntity>(IEnumerable<object> ids)
            where TEntity : class
        {
            return await _repository.DeleteByIdAsync<TEntity>(ids);
        }

        public virtual async Task<int> DeleteAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.DeleteAsync<TEntity>(Mapper.Map<Expression<Func<TEntity, bool>>>(predicate));
        }

        public virtual int Update<TEntity, TDto>(object id, TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = _repository.FindById<TEntity>(id);
            Mapper.Map<TDto, TEntity>(dto, entity);

            return _repository.Update<TEntity>(id, entity);
        }

        public virtual async Task<int> UpdateSelectFieldsAsync<TEntity, TDto>(object id, TDto newDto, params Expression<Func<TEntity, Object>>[] propertiesToUpdate)
            where TEntity : class
            where TDto : class
        {

            if (propertiesToUpdate == null)
            {
                throw new ArgumentException("propertiesToUpdate is null");
            }

            if (newDto == null)
            {
                throw new ArgumentException("dto is null");
            }

            var newEntity = _repository.FindById<TEntity>(id);
            if (newEntity == null)
            {
                throw new ArgumentException("entity does not exist in current context");
            }

            Mapper.Map<TDto, TEntity>(newDto, newEntity);

            var dbContext = GetDbContext<DbContext>();

            var attachedEntity = _repository.FindById<TEntity>(id);
            dbContext.Set<TEntity>().Attach(attachedEntity);
            var entry = dbContext.Entry(attachedEntity);

            if (entry == null)
            {
                throw new ArgumentException(attachedEntity.ToString() + " does not exist in current context");
            }

            Type newType = newEntity.GetType();

            foreach (var item in propertiesToUpdate)
            {
                MemberExpression memExp = item.Body as MemberExpression;
                if (memExp == null)
                {
                    UnaryExpression unExp = item.Body as UnaryExpression;
                    if (unExp == null)
                    {
                        throw new InvalidOperationException("Selected property " + item.Name + " is invalid");
                    }
                    memExp = (MemberExpression)unExp.Operand;
                }

                string propertyName = memExp.Member.Name;
                Object newValue = newType.GetProperties().Single(x => x.Name == propertyName).GetValue(newEntity, null);
                var propertyEntry = entry.Property(propertyName);
                propertyEntry.CurrentValue = newValue;
                propertyEntry.IsModified = true;
            }

            return await dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> UpdateAsync<TEntity, TDto>(object id, TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = await _repository.FindByIdAsync<TEntity>(id);
            Mapper.Map<TDto, TEntity>(dto, entity);

            return await _repository.UpdateAsync<TEntity>(id, entity);
        }

        public void UpdateRelationship<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class
        {
            _repository.UpdatRelationship<TEntity, TRelationship>(entityId, relationShipFilter, relationshipSelector);
        }

        public async Task UpdateRelationshipAsync<TEntity, TRelationship>(object entityId, Expression<Func<TRelationship, bool>> relationShipFilter, Func<TEntity, ICollection<TRelationship>> relationshipSelector)
            where TEntity : class
            where TRelationship : class
        {
            await _repository.UpdatRelationshipAsync<TEntity, TRelationship>(entityId, relationShipFilter, relationshipSelector);
        }

        public virtual int ExecuteNonQuery(string query, params object[] parameters)
        {
            return _repository.ExecuteNonQuery(query, parameters);
        }

        public virtual TResult ExecuteReader<TResult>(string query, params object[] parameters)
        {
            return _repository.ExecuteReader<TResult>(query, parameters);
        }

        public virtual TContext GetDbContext<TContext>() where TContext : class
        {
            return _repository.GetDbContext<TContext>();
        }

        public virtual ITransaction BeginTransaction()
        {
            return _repository.BeginTransaction();
        }

        public virtual ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return _repository.BeginTransaction(isolationLevel);
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
                _repository = null;
            }
        }

        #endregion

        private void MapKeyBack<TEntity, TDto>(TEntity entity, TDto dto)
            where TEntity : class
            where TDto : class
        {
            try
            {
                var entityKey = typeof(TEntity).GetProperties().Where(x => Attribute.IsDefined(x, typeof(System.ComponentModel.DataAnnotations.KeyAttribute))).SingleOrDefault();

                if (entityKey == null)
                {
                    return;
                }

                var dtoKey = typeof(TDto).GetProperty(entityKey.Name);

                if (dtoKey == null)
                {
                    dtoKey = typeof(TDto).GetProperties().Where(x => Attribute.IsDefined(x, typeof(System.ComponentModel.DataAnnotations.KeyAttribute))).SingleOrDefault();
                }

                if (dtoKey != null)
                {
                    dtoKey.SetValue(dto, entityKey.GetValue(entity));
                }
            }
            catch (InvalidOperationException)
            {
                // Do nothing as key is not specified;
            }
        }

        public void BeginAuditLog()
        {
            _repository.BeginAuditLog();
        }

        public string GetLastLog()
        {
            return _repository.GetLastLog();
        }
    }

    public class BaseService<TRepository> : BaseService where TRepository : BGP.Utils.Repository.EntityFramework.Repository
    {
        public BaseService(TRepository repository) : base(repository)
        {
        }
    }
}
