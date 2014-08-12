using System.Linq;

namespace SingledOut.Repository.QueryBuilders.User
{
    public interface IQueryBuilder
    {
        IQueryable<Data.User> BuildQuery(SearchParameters.UsersSearchParameters sp);
    }
}
