using SingledOut.Data.Entities;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserModelFactory
    {
        UserModel Create(User user);

        User Parse(UserModel model);
    }
}
