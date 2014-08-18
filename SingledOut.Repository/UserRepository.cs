using System;
using System.Linq;
using SingledOut.Data;
using SingledOut.Repository.QueryBuilders.User;
using SingledOut.SearchParameters;
using SingledOut.Services.Services;

namespace SingledOut.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly SingledOutEntities _ctx;
        private readonly Security _security;
        private readonly IQueryBuilder _queryBuilder;

        public UserRepository(
            SingledOutEntities ctx, 
            Security security,
            IQueryBuilder queryBuilder)
            : base(ctx)
        {
            _ctx = ctx;
            _security = security;
            _queryBuilder = queryBuilder;
        }

        public IQueryable<User> Search(UsersSearchParameters sp)
        {
            return  _queryBuilder.BuildQuery(sp);
        }

        public IQueryable<User> GetAllUsers()
        {
            return _ctx.Users.AsQueryable();
        }

        public User GetUser(int userID)
        {
            return _ctx.Users.SingleOrDefault(o => o.ID == userID);
        }

        public User Insert(User user)
        {
            var result = 0;

            // Check if the username or Facebook username already exists.
            var existingUser = false;
            var facebookExistingUser = false;

            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser = GetAllUsers().Any(o => o.Email == user.Email);
            }
            User facebookUser = null;
            if (!string.IsNullOrEmpty(user.FacebookUserName))
            {
                facebookUser = GetAllUsers().SingleOrDefault(o => o.FacebookUserName == user.FacebookUserName && o.Email == user.Email);
                facebookExistingUser = facebookUser != null;
            }

            if (!existingUser && !facebookExistingUser)
            {
                // Create auth token for user.
                user.AuthToken = Guid.NewGuid();

                _ctx.Users.Add(user);
                result = SaveAll();
                return user;
            }
            else
            {
                if (!facebookExistingUser)
                {
                    user.ID = -1;
                }
                else
                {
                    return facebookUser;
                }
            }
            
            return user;
        }

        public int Update(User originalUser, User updatedUser)
        {
            _ctx.Entry(originalUser).CurrentValues.SetValues(updatedUser);
           
            return SaveAll();
        }

        public int DeleteUser(int id)
        {
            var user = GetUser(id);
            _ctx.Users.Remove(user);
            return SaveAll();
        }

        public bool LoginUser(string email, string password)
        {
            var user = _ctx.Users.SingleOrDefault(s => s.Email == email);

            if (user != null)
            {
                if (_security.MatchHash(user.Password, password))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
