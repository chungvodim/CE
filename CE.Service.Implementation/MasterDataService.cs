using CE.Repository.Main;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Dto;
using CE.Entity.Main;
using System.Data.Entity;
using CE.Entity.Log;

namespace CE.Service.Implementation
{
    public class MasterDataService : BaseService, IMasterDataService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MasterDataService));

        public MasterDataService(BGP.Utils.Service.EntityFramework.BaseService<MainEntityFrameworkRepository> mainService) : base(mainService, _logger) { }
    }
}
