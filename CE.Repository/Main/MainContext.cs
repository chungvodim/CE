namespace CE.Repository.Main
{
    using Entity.Main;
    using EntityFramework.Audit;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class MainContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        static MainContext()
        {
            // IMPORTANT!!!
            // Don't use this line to re-create DB, it may delete translated resources table
            Database.SetInitializer(new MainDatabaseInitializer());
            // --------------------------------------------------------
            //Database.SetInitializer<MainContext>(null);
        }

        // Your context has been configured to use a 'BEGAIT' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'CE.Repository.Main' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'BEGAIT' 
        // connection string in the application configuration file.
        public MainContext()
            : base("name=Main")
        {
#if DEBUG
            Database.Log = x => { System.Diagnostics.Debug.WriteLine(x); };
#endif
        }

        public static MainContext Create()
        {
            return new MainContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Audit log
            var audit = AuditConfiguration.Default;
            audit.IncludeRelationships = false;
            audit.LoadRelationships = false;
            audit.DefaultAuditable = false;

            audit.IsAuditable<User>();
            audit.IsAuditable<UserRole>();

            // Change Aspnet identity tables' name
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<UserRole>().ToTable("UserRoles");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogins");

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();


            modelBuilder.Properties()
                .Where(x => x.GetCustomAttributes(false).OfType<NonUnicodeAttribute>().Any())
                .Configure(c => c.IsUnicode(false));

        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

    }
}