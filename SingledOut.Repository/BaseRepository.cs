using System;
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
            var result = 0;
            try
            {
                result = _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            return result;
        }
    }
}
