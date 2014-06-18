using System.Linq;
using SingledOut.Data.Entities;
using SingledOut.SearchParameters;

namespace SingledOut.Repository
{
    public interface IUserRepository
    {
        IQueryable<User> GetAllUsers();

        User GetUser(int userID);

        int Insert(User user);

        int Update(User originalUser, User updatedUser);

        int DeleteUser(int id);

        int SaveAll();

        bool LoginUser(string email, string password);

        IQueryable<User> Search(UsersSearchParameters sp);
    }
}
