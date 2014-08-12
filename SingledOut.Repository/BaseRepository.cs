using SingledOut.Data;

namespace SingledOut.Repository
{
    public abstract class BaseRepository
    {
        private readonly SingledOutEntities _ctx;

        protected BaseRepository(SingledOutEntities ctx)
        {
            _ctx = ctx;
        }

        public int SaveAll()
        {
            return _ctx.SaveChanges();
        }
    }
}
