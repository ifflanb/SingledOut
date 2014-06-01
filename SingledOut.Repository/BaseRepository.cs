using SingledOut.Data;

namespace SingledOut.Repository
{
    public abstract class BaseRepository
    {
        private readonly SingledOutContext _ctx;

        protected BaseRepository(SingledOutContext ctx)
        {
            _ctx = ctx;
        }

        public int SaveAll()
        {
            return _ctx.SaveChanges();
        }
    }
}
