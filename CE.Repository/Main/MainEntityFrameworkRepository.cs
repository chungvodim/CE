namespace CE.Repository.Main
{
    public class MainEntityFrameworkRepository : BGP.Utils.Repository.EntityFramework.Repository
    {
        public MainEntityFrameworkRepository(MainContext dbContext) : base(dbContext)
        {
        }
    }
}
