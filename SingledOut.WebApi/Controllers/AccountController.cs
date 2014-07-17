using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using SingledOut.Model;
using SingledOut.WebApi.Filters;

namespace SingledOut.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private readonly UsersController _usersController;

        public AccountController(UsersController usersController)
        {
            _usersController = usersController;
        }

        [HttpPost]
        public HttpResponseMessage Register([FromBody] UserModel userModel)
        {
            _usersController.Request = Request;
            return _usersController.Post(userModel);
        }

        [HttpGet]
        [SingledOutAuthorization]
        public HttpResponseMessage Login()
        {
            _usersController.Request = Request;
            var email = Thread.CurrentPrincipal.Identity.Name;
            var user = _usersController.Get().Single(o => o.Email == email);

            var result = Request.CreateResponse(HttpStatusCode.Accepted, user);
            return result;
        }

        [HttpGet]
        public HttpResponseMessage RetrievePassword(string email)
        {
            var user = _usersController.Get().SingleOrDefault(o => o.Email == email);
            if (user != null && !string.IsNullOrEmpty(user.Password))
            {
                // _email.SendEmail()
            }
            var result = Request.CreateResponse(HttpStatusCode.OK, user);

            return result;
        }
    }
}
