using System;
using System.Data.Entity;
using System.Linq;
using SingledOut.Data;
using SingledOut.Model;

namespace SingledOut.Repository.QueryBuilders.User
{
    public class QueryBuilder : IQueryBuilder
    {
        private const string Male = "male";
        private const string Female = "female";
        private const int EarthsRadius = 6371;
        
        /// <summary>
        /// Get Users based on search parameters.
        /// </summary>
        /// <param name="sp">The User search parameters.</param>
        public IQueryable<Data.User> BuildQuery(SearchParameters.UsersSearchParameters sp)
        {

            IQueryable<Data.User> query = null;

            var ctx = new SingledOutEntities();
            
            query = from u in ctx.Users
                select u;

            if (!string.IsNullOrEmpty(sp.FacebookUserName))
            {
                query = from u in query
                    where u.FacebookUserName == sp.FacebookUserName
                    select u;
            }

            if (sp.AgeFrom.HasValue && sp.AgeTo.HasValue)
            {
                query = query.Where(u => u.Age >= sp.AgeFrom && u.Age <= sp.AgeTo);
            }

            if (sp.Sex.HasValue)
            {
                if (sp.Sex == GenderEnum.Male)
                {
                    query = query.Where(u => u.Sex == Male);
                }
                if (sp.Sex == GenderEnum.Female)
                {
                    query = query.Where(u => u.Sex == Female);
                }

                if (sp.Sex == GenderEnum.Both)
                {
                    query = query.Where(u => u.Sex == Female || u.Sex == Male);
                }
            }

            //IQueryable<Data.Entities.User> users = null;
            if (sp.Distance.HasValue && sp.UserLatitude.HasValue && sp.UserLongitude.HasValue)
            {
                query = (from u in query.AsEnumerable()
                    join ul in ctx.UserLocations on u.ID equals ul.UserID
                    where
                        Math.Acos(Math.Cos(ToRadians(90 - (double) sp.UserLatitude))*
                                    Math.Cos(ToRadians(90 - ul.Latitude)) +
                                    Math.Sin(ToRadians(90 - (double) sp.UserLatitude))*
                                    Math.Sin(ToRadians(90 - ul.Latitude))*
                                    Math.Cos(ToRadians((double) sp.UserLongitude - ul.Longitude)))*EarthsRadius <=
                        ((double) sp.Distance / 1000)
                    select u).AsQueryable();
            }

            query = query.Include(o => o.UserLocation);

            query = query.OrderBy(u => u.FirstName).ThenBy(u => u.Surname);

            return query;
        }

        /// <summary>
        /// Convert to Radians.
        /// </summary>
        /// <param name="val">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        public double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
