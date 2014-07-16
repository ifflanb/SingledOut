using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using SingledOut.Model;

namespace SingledOut.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private readonly UsersController _usersController;

        public AccountController(UsersController usersController)
        {
            _usersController = usersController;
        }

        public override System.Threading.Tasks.Task<HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }

        [HttpPost]
        public HttpResponseMessage Register([FromBody] UserModel userModel)
        {
            return _usersController.Post(userModel);
        }

        [HttpGet]
        public HttpResponseMessage Login()
        {
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
