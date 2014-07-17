using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using SingledOut.Model;
using SingledOut.Repository;
using SingledOut.SearchParameters;
using SingledOut.WebApi.Interfaces;

namespace SingledOut.WebApi.Controllers
{
    public class UsersSearchController : ApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserModelFactory _userModelFactory;

        public UsersSearchController(IUserRepository userRepository,
            IUserModelFactory userModelFactory)
        {
            _userRepository = userRepository;
            _userModelFactory = userModelFactory;
        }

        public IEnumerable<UserModel> Get([FromUri] UsersSearchParameters sp)
        {
            var query = _userRepository.Search(sp);

            var results = query.ToList().Select(s => _userModelFactory.Create(s));

            return results;
        }
    }
}
