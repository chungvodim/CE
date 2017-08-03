namespace CE.Repository.Log
{
    public class LogEntityFrameworkRepository : BGP.Utils.Repository.EntityFramework.Repository
    {
        public LogEntityFrameworkRepository(LogContext dbContext) : base(dbContext)
        {
        }
    }
}
