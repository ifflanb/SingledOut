using System.Net.Http;
using SingledOut.Data.Entities;
using SingledOut.Model;

namespace SingledOut.WebApi.Interfaces
{
    public interface IUserModelFactory
    {
        UserModel Create(User user, HttpRequestMessage request);

        User Parse(UserModel model);
    }
}
