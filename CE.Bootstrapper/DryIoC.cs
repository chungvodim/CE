using CE.Repository;
using CE.Repository.Main;
//using CE.Repository.Mongo;
using CE.Repository.Log;
using BGP.Utils.CacheManager;
using BGP.Utils.CacheManager.MemoryCache;
using DryIoc;
using MongoDB.Driver;
using System;
using System.Data.Entity;
using CE.Repository.Mongo;
using BGP.Utils.Repository.EntityFramework;

namespace CE.Bootstrapper
{
    public class DryIoC
    {
        public static void Configure(IContainer container, IReuse defaultReuse = null)
        {
            // Register all the services interface/implement here
            // The services used by clients should be abstract. DryIoC will be the one manage the actual implementation
            // Service that used Sql and MongoDb should be separated to avoid confusion
            
            // Choosing cache manager
            container.RegisterDelegate<ICacheManager>(x => new CacheManager("BEGAITCache"), Reuse.Singleton);

            // Sql EF services
            container.Register<MainContext>(defaultReuse);
            container.Register<MainEntityFrameworkRepository>(defaultReuse);
            container.Register<BGP.Utils.Service.EntityFramework.BaseService<MainEntityFrameworkRepository>>(defaultReuse);
            //container.Register<IAdminstrationService, AdministrationService>(defaultReuse);
            //container.Register<IMasterDataService, MasterDataService>(defaultReuse);

            // Log service
            container.Register<LogContext>(defaultReuse);
            container.Register<LogEntityFrameworkRepository>(defaultReuse);
            container.Register<BGP.Utils.Service.EntityFramework.BaseService<LogEntityFrameworkRepository>>(defaultReuse);
            //container.Register<ILogService, LogService>(defaultReuse);

            // Mongo service
            container.RegisterDelegate<IMongoDatabase>(x => MongoDatabaseFactory.Create(), defaultReuse);
            container.Register<MongoRepository>(defaultReuse);
            container.Register<BGP.Utils.Service.Mongo.BaseService<MongoRepository>>(defaultReuse);
        }
    }
}
