using System.Linq;
using SingledOut.Data;
using SingledOut.Data.Entities;
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
            if (!string.IsNullOrEmpty(user.Username))
            {
                existingUser = GetAllUsers().Any(o => o.Username == user.Username);
            }
            if (!string.IsNullOrEmpty(user.FacebookUserName))
            {
                existingUser = GetAllUsers().Any(o => o.FacebookUserName == user.FacebookUserName);
            }

            if (!existingUser)
            {
                _ctx.Users.Add(user);
                result = SaveAll();
            }
            else
            {
                result = -1;
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

        public bool LoginUser(string userName, string password)
        {
            var user = _ctx.Users.SingleOrDefault(s => s.Username == userName);

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
