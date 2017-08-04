using CE.Dto;
//using CE.Entity.Mongo;
using CE.Enum;
using CE.Repository.Main;
using BGP.Utils.Service;
using EntityFramework.Audit;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CE.Service.Implementation
{
    public abstract class BaseService : IDisposable
    {
        protected IAdvanceBaseService _mainService;
        protected IAdvanceBaseService _dataService;
        protected IBaseService _logService;
        private ILog _logger;

        protected BaseService(IAdvanceBaseService baseService, ILog logger)
        {
            _mainService = baseService;
            _logger = logger;

            if (_mainService != null)
            {
                _mainService.BeginAuditLog();
            }
        }

        protected BaseService(IAdvanceBaseService mainService, IBaseService logService, ILog logger)
        {
            _mainService = mainService;
            _logService = logService;
            _logger = logger;

            if(_mainService != null)
            {
                _mainService.BeginAuditLog();
            }
        }

        protected BaseService(IAdvanceBaseService mainService, IBaseService logService, IAdvanceBaseService dataService, ILog logger)
        {
            _mainService = mainService;
            _dataService = dataService;
            _logService = logService;
            _logger = logger;

            if (_mainService != null)
            {
                _mainService.BeginAuditLog();
            }
            if (_mainService != null)
            {
                _dataService.BeginAuditLog();
            }
        }

        protected void Processing(IBaseService service, Action<IBaseService> action)
        {
            try
            {
                // Wrap action in a scope
                using (var ts = service.BeginTransaction())
                {
                    action.Invoke(service);

                    ts.Commit();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                throw ex;
            }
        }

        protected TResult Processing<TResult>(IBaseService service, Func<IBaseService, TResult> action)
        {
            try
            {
                // Wrap action in a scope
                using (var ts = service.BeginTransaction())
                {
                    var result = action.Invoke(service);

                    ts.Commit();
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                throw ex;
            }
        }

        protected TResult AdvanceProcessing<TResult>(IAdvanceBaseService service, Func<IAdvanceBaseService, TResult> action)
        {
            try
            {
                // Wrap action in a scope
                using (var ts = service.BeginTransaction())
                {
                    var result = action.Invoke(service);

                    ts.Commit();
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                throw ex;
            }
        }

        protected async Task ProcessingAsync(IBaseService service, Func<IBaseService, Task> action)
        {
            try
            {
                // Wrap action in a scope
                using (var ts = service.BeginTransaction())
                {
                    await action.Invoke(service);

                    ts.Commit();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                throw ex;
            }
        }

        protected async Task AdvanceProcessingAsync(IAdvanceBaseService service, Func<IAdvanceBaseService, Task> action)
        {
            try
            {
                // Wrap action in a scope
                using (var ts = service.BeginTransaction())
                {
                    await action.Invoke(service);

                    ts.Commit();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                throw ex;
            }
        }

        protected async Task<TResult> ProcessingAsync<TResult>(IBaseService service, Func<IBaseService, Task<TResult>> action)
        {
            try
            {
                // Wrap action in a scope
                using (var ts = service.BeginTransaction())
                {
                    var result = await action.Invoke(service);

                    ts.Commit();
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                if (_logger != null)
                {
                    _logger.Error(ex.Message, ex);
                }

                throw ex;
            }
        }

        public void Dispose()
        {
            if (_mainService != null)
            {
                _mainService.Dispose();
                _mainService = null;
            }
            if (_dataService != null)
            {
                _dataService.Dispose();
                _dataService = null;
            }
            if (_logService != null)
            {
                _logService.Dispose();
                _logService = null;
            }
        }
    }
}
