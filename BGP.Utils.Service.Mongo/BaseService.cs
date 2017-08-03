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
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using EntityFramework.Audit;
using Newtonsoft.Json;
using DeepEqual.Syntax;

namespace BGP.Utils.Service.Mongo
{
    public class BaseService : IAdvanceBaseService
    {
        protected BGP.Utils.Repository.Mongo.Repository _repository;

        private bool _isAuditLog;
        private ChangeLog _changeLog;
        private object _originalEntityObject;
        private object _newEntityObject;

        public BaseService(BGP.Utils.Repository.Mongo.Repository repository)
        {
            _repository = repository;
        }

        #region IBaseService Members

        public virtual List<TDto> All<TEntity, TDto>()
            where TEntity : class
        {
            return _repository.Collection<TEntity>().Find(x => true).Project(x => Mapper.Map<TDto>(x)).ToList();
        }

        public async virtual Task<List<TDto>> AllAsync<TEntity, TDto>()
            where TEntity : class
        {
            return await _repository.Collection<TEntity>().Find(x => true).Project(x => Mapper.Map<TDto>(x)).ToListAsync();
        }

        public virtual int Count<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Count(Transform<TEntity, TDto>(predicate));
        }

        public virtual async Task<int> CountAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.CountAsync(Transform<TEntity, TDto>(predicate));
        }

        public virtual TDto Find<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Collection<TEntity>().Find(Transform<TEntity, TDto>(predicate)).Project(x => Mapper.Map<TDto>(x)).FirstOrDefault();
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
            return await _repository.Collection<TEntity>().Find(Transform<TEntity, TDto>(predicate)).Project(x => Mapper.Map<TDto>(x)).FirstOrDefaultAsync();
        }

        public virtual IQueryable<TDto> Query<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Collection<TEntity>().Find(Transform<TEntity, TDto>(predicate)).Project(x => Mapper.Map<TDto>(x)).ToEnumerable().AsQueryable();
        }

        public virtual List<TDto> Filter<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Collection<TEntity>().Find(Transform<TEntity, TDto>(predicate)).Project(x => Mapper.Map<TDto>(x)).ToList();
        }

        public async virtual Task<List<TDto>> FilterAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.Collection<TEntity>().Find(Transform<TEntity, TDto>(predicate)).Project(x => Mapper.Map<TDto>(x)).ToListAsync();
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

            var query = _repository.Collection<TEntity>()
                .Find(Transform<TEntity, TDto>(pagingParams.Predicate));

            result.ItemCount = (int)query.Count();

            if (pagingParams.SortBy != null)
            {
                query = pagingParams.IsAscending ? query.SortBy(Transform<TEntity, TDto>(pagingParams.SortBy)) : query.SortByDescending(Transform<TEntity, TDto>(pagingParams.SortBy));
            }
            else
            {
                query = query.Sort(pagingParams.IsAscending ? _repository.Sorter<TEntity>().Ascending(pagingParams.SortField) : _repository.Sorter<TEntity>().Descending(pagingParams.SortField));
            }

            if (pagingParams.StartingIndex > 0)
            {
                query = query.Skip(pagingParams.StartingIndex);
            }

            if (pagingParams.PageSize > 0)
            {
                query = query.Limit(pagingParams.PageSize);
            }

            result.Items = query.Project(x => Mapper.Map<TEntity, TDto>(x)).ToList();

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

            var query = _repository.Collection<TEntity>()
                .Find(Transform<TEntity, TDto>(pagingParams.Predicate));

            result.ItemCount = (int)await query.CountAsync();

            if (pagingParams.SortBy != null)
            {
                query = pagingParams.IsAscending ? query.SortBy(Transform<TEntity, TDto>(pagingParams.SortBy)) : query.SortByDescending(Transform<TEntity, TDto>(pagingParams.SortBy));
            }
            else
            {
                query = query.Sort(pagingParams.IsAscending ? _repository.Sorter<TEntity>().Ascending(pagingParams.SortField) : _repository.Sorter<TEntity>().Descending(pagingParams.SortField));
            }

            if (pagingParams.StartingIndex > 0)
            {
                query = query.Skip(pagingParams.StartingIndex);
            }

            if (pagingParams.PageSize > 0)
            {
                query = query.Limit(pagingParams.PageSize);
            }

            result.Items = await query.Project(x => Mapper.Map<TEntity, TDto>(x)).ToListAsync();

            return result;
        }

        public virtual bool Contain<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return _repository.Contain(Transform<TEntity, TDto>(predicate));
        }

        public virtual async Task<bool> ContainAsync<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
            where TEntity : class
        {
            return await _repository.ContainAsync(Transform<TEntity, TDto>(predicate));
        }

        public virtual void Create<TEntity, TDto>(TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = Mapper.Map<TDto, TEntity>(dto);
            SetUpAuditLog(AuditAction.Added, entity);

            _repository.Create<TEntity>(entity);

            if (Mapper.Configuration.FindTypeMapFor<TEntity, TDto>() != null)
            {
                Mapper.Map(entity, dto);
            }
        }

        public virtual void CreateMany<TEntity, TDto>(IEnumerable<TDto> dtos)
            where TEntity : class
            where TDto : class
        {
            var entities = Mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(dtos);

            _repository.CreateMany<TEntity>(entities);

            if (Mapper.Configuration.FindTypeMapFor<TEntity, TDto>() != null)
            {
                var listEntities = entities.ToList();
                var listDto = dtos.ToList();
                for (int i = 0; i < listEntities.Count; i++)
                {
                    Mapper.Map(listEntities[i], listDto[i]);
                }
            }
        }

        public async virtual Task CreateAsync<TEntity, TDto>(TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = Mapper.Map<TDto, TEntity>(dto);
            SetUpAuditLog(AuditAction.Added, entity);

            await _repository.CreateAsync<TEntity>(entity);

            if (Mapper.Configuration.FindTypeMapFor<TEntity, TDto>() != null)
            {
                Mapper.Map(entity, dto);
            }
        }

        public async virtual Task CreateManyAsync<TEntity, TDto>(IEnumerable<TDto> dtos)
            where TEntity : class
            where TDto : class
        {
            var entities = Mapper.Map<IEnumerable<TDto>, IEnumerable<TEntity>>(dtos);

            await _repository.CreateManyAsync<TEntity>(entities);

            if (Mapper.Configuration.FindTypeMapFor<TEntity, TDto>() != null)
            {
                var listEntities = entities.ToList();
                var listDto = dtos.ToList();
                for (int i = 0; i < listEntities.Count; i++)
                {
                    Mapper.Map(listEntities[i], listDto[i]);
                }
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
            return _repository.Delete<TEntity>(Transform<TEntity, TDto>(predicate));
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
            return await _repository.DeleteAsync<TEntity>(Transform<TEntity, TDto>(predicate));
        }

        public virtual int Update<TEntity, TDto>(object id, TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = _repository.FindById<TEntity>(id);
            SetUpAuditLog(AuditAction.Modified, entity);

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

            var newEntity = await _repository.FindByIdAsync<TEntity>(id);
            if (newEntity == null)
            {
                throw new ArgumentException("entity does not exist in current context");
            }
            SetUpAuditLog(AuditAction.Modified, newEntity);

            Mapper.Map<TDto, TEntity>(newDto, newEntity);

            Type newType = newEntity.GetType();

            var entity = await _repository.FindByIdAsync<TEntity>(id);

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
                var propInfo = newType.GetProperties().Single(x => x.Name == propertyName);
                if(propInfo.CanRead && propInfo.CanWrite)
                {
                    Object newValue = propInfo.GetValue(newEntity, null);
                    propInfo.SetValue(entity, newValue);
                }
            }

            return await _repository.UpdateAsync<TEntity>(id, entity);
        }

        public virtual async Task<int> UpdateAsync<TEntity, TDto>(object id, TDto dto)
            where TEntity : class
            where TDto : class
        {
            var entity = await _repository.FindByIdAsync<TEntity>(id);
            SetUpAuditLog(AuditAction.Modified, entity);

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

        private Expression<Func<TEntity, bool>> Transform<TEntity, TDto>(Expression<Func<TDto, bool>> predicate)
        {
            return Mapper.Map<Expression<Func<TEntity, bool>>>(predicate);
        }

        private Expression<Func<TEntity, object>> Transform<TEntity, TDto>(Expression<Func<TDto, object>> predicate)
        {
            return Mapper.Map<Expression<Func<TEntity, object>>>(predicate);
        }

        private void SetUpAuditLog<T>(AuditAction action, T entity)
        {
            if (_isAuditLog)
            {
                _changeLog = new ChangeLog { AuditAction = action };

                _originalEntityObject = action == AuditAction.Added ? Activator.CreateInstance<T>() : entity.DeepCopyByExpressionTree();
                _newEntityObject = entity;
            }
        }

        public void BeginAuditLog()
        {
            _isAuditLog = true;
        }

        public string GetLastLog()
        {
            var properties = _newEntityObject.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (_changeLog.Properties.FirstOrDefault(m => m.Name == prop.Name) != null)
                    continue;

                var newValue = prop.GetValue(_newEntityObject);
                var oldValue = prop.GetValue(_originalEntityObject);

                if (_changeLog.AuditAction == AuditAction.Added || oldValue.IsDeepEqual(newValue) == false)
                {
                    _changeLog.Properties.Add(new ChangeLogProperty
                    {
                        Name = prop.Name,
                        New = newValue,
                        Old = oldValue
                    });
                }
            }

            if (_changeLog.Properties.Count == 0)
                return null;

            var idProp = properties.FirstOrDefault(m => m.Name.ToLower() == "id");
            if (idProp != null && _changeLog.Keys.Any(m => m.Name == "Id") == false)
            {
                _changeLog.Keys.Add(new ChangeLogKey()
                {
                    Name = "Id",
                    Value = idProp.GetValue(_newEntityObject)
                });
            }

            return JsonConvert.SerializeObject(_changeLog);
        }
    }

    public class BaseService<TRepository> : BaseService where TRepository : BGP.Utils.Repository.Mongo.Repository
    {
        public BaseService(TRepository repository) : base(repository)
        {
        }
    }
}
