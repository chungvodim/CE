using System.Collections.Generic;
using System.Threading.Tasks;
using CE.Dto;
using BGP.Utils.CacheManager;
using CE.Repository.Main;
using CE.Service.Implementation;
using CE.Repository.Log;
using CE.Service;

namespace CE.Service.Cache
{
    public class CacheAdministrationService : AdministrationService, IAdminstrationService
    {
        private ICacheManager _cacheManager;

        public CacheAdministrationService(BGP.Utils.Service.EntityFramework.BaseService<MainEntityFrameworkRepository> baseService,
            BGP.Utils.Service.EntityFramework.BaseService<LogEntityFrameworkRepository> logService,
            ICacheManager cacheManager) : base(baseService, logService)
        {
            _cacheManager = cacheManager;
        }

        public async override Task<List<RoleDto>> GetAllRoles()
        {
            return await _cacheManager.GetOrUpdateAsync("_GetAllRoles", async () => { return await base.GetAllRoles(); }, 30);
        }
    }
}
