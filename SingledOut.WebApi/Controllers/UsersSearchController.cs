using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        [HttpGet]
        public HttpResponseMessage Search([FromUri] UsersSearchParameters sp)
        {
            try
            {
                var users = _userRepository.Search(sp).ToList();
                
                if (users.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, _userModelFactory.Create(users, sp));
                }
                return Request.CreateResponse(HttpStatusCode.OK, new List<UserModel>());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}
