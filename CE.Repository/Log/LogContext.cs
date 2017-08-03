using CE.Entity.Log;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Repository.Log
{
    public class LogContext : DbContext
    {
        static LogContext()
        {
            // IMPORTANT!!!
            // Don't use this line to re-create DB, it may delete translated resources table
            //Database.SetInitializer(new BEGAITLogDatabaseInitializer());
            // --------------------------------------------------------
            Database.SetInitializer<LogContext>(null);
        }

        // Your context has been configured to use a 'BEGAIT' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'BGP.Repository.BEGAIT' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'BEGAIT' 
        // connection string in the application configuration file.
        public LogContext()
            : base("name=Log")
        {
#if DEBUG
            Database.Log = x => { System.Diagnostics.Debug.WriteLine(x); };
#endif
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<UserLoginLog> UserLoginLogs { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }
    }
}
