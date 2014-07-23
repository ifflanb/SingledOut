using System.Collections.Generic;
using System.Linq;

namespace SingledOut.Repository.QueryBuilders.User
{
    public interface IQueryBuilder
    {
        IQueryable<Data.Entities.User> BuildQuery(SearchParameters.UsersSearchParameters sp);
    }
}
