using System.Linq;
using SingledOut.Data;
using SingledOut.Data.Entities;
using SingledOut.SearchParameters;
using SingledOut.Services.Services;

namespace SingledOut.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly SingledOutContext _ctx;
        private readonly Security _security;

        public UserRepository(
            SingledOutContext ctx, 
            Security security)
            : base(ctx)
        {
            _ctx = ctx;
            _security = security;
        }

        public IQueryable<User> Search(UsersSearchParameters sp)
        {
            var query = from o in _ctx.Users
                        select o;

            if (!string.IsNullOrEmpty(sp.FacebookUserName))
            {
                query = from o in query
                    where o.FacebookUserName == sp.FacebookUserName 
                    select o;
            }

            //if (!string.IsNullOrEmpty(sp.Username))
            //{
            //    query = from o in query
            //            where o.Username == sp.Username
            //            select o;
            //}

            //if (!string.IsNullOrEmpty(sp.Sex))
            //{
            //    query = from o in query
            //            where o.FacebookUserName == sp.Sex
            //            select o;
            //}

            //if (!string.IsNullOrEmpty(sp.Surname))
            //{
            //    query = from o in query
            //            where o.FacebookUserName == sp.Surname
            //            select o;
            //}

            //if (!string.IsNullOrEmpty(sp.FirstName))
            //{
            //    query = from o in query
            //            where o.FacebookUserName == sp.FirstName
            //            select o;
            //}

            return query.AsQueryable();
        }

        public IQueryable<User> GetAllUsers()
        {
            return _ctx.Users.AsQueryable();
        }

        public User GetUser(int userID)
        {
            return _ctx.Users.SingleOrDefault(o => o.ID == userID);
        }

        public int Insert(User user)
        {
            var result = 0;

            // Check if the username or Facebook username already exists.
            var existingUser = false;
            var facebookExistingUser = false;

            if (!string.IsNullOrEmpty(user.Email))
            {
                existingUser = GetAllUsers().Any(o => o.Email == user.Email);
            }
            if (!string.IsNullOrEmpty(user.FacebookUserName))
            {
                facebookExistingUser = GetAllUsers().Any(o => o.FacebookUserName == user.FacebookUserName);
            }

            if (!existingUser && !facebookExistingUser)
            {
                _ctx.Users.Add(user);
                result = SaveAll();
            }
            else
            {
                if (!facebookExistingUser)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            
            return result;
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
